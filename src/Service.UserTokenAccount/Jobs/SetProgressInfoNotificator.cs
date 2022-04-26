using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Services;
using Service.Education.Extensions;
using Service.ServiceBus.Models;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Services;

namespace Service.UserTokenAccount.Jobs
{
	/// <summary>
	///     Завершение дисциплины (80%, 100%)
	/// </summary>
	public class SetProgressInfoNotificator : NotificatorBase<SetProgressInfoNotificator>
	{
		private readonly string _settingsKey = Program.Settings.KeyTutorialProgressPrc;

		private readonly IServerKeyValueDtoRepository<TutorialProgressPrcDto[]> _serverKeyValueDtoRepository;

		public SetProgressInfoNotificator(ILogger<SetProgressInfoNotificator> logger,
			ISubscriber<IReadOnlyList<SetProgressInfoServiceBusModel>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock, IServerKeyValueDtoRepository<TutorialProgressPrcDto[]> serverKeyValueDtoRepository) :
				base(accountRepository, operationRepository, logger, systemClock)
		{
			_serverKeyValueDtoRepository = serverKeyValueDtoRepository;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<SetProgressInfoServiceBusModel> events)
		{
			foreach (SetProgressInfoServiceBusModel message in events)
			{
				int tutorialProgress = message.TutorialProgress;

				if (!message.SetUserProgress || !tutorialProgress.IsOkProgress())
					continue;

				string userId = message.UserId;

				List<TutorialProgressPrcDto> dtos = (await _serverKeyValueDtoRepository.Get(_settingsKey, userId) 
					?? Array.Empty<TutorialProgressPrcDto>())
					.ToList();

				TutorialProgressPrcDto dto = dtos.FirstOrDefault(dto => dto.Tutorial == message.Tutorial);
				if (dto == null)
				{
					dto = new TutorialProgressPrcDto(){Tutorial = message.Tutorial};
					dtos.Add(dto);
				}

				if (dto.SetOkPrc && dto.SetMaxPrc)
					continue;

				int value;
				if (tutorialProgress.IsMaxProgress() && !dto.SetMaxPrc)
				{
					dto.SetMaxPrc = true;
					value = GetSettings().Invoke().TutorialFinished100;
				}
				else if (tutorialProgress.IsOkProgress() && !dto.SetOkPrc)
				{
					dto.SetOkPrc = true;
					value = GetSettings().Invoke().TutorialFinished80;
				}
				else continue;

				bool dtoSaved = await _serverKeyValueDtoRepository.Save(_settingsKey, userId, dtos.ToArray());
				if (!dtoSaved)
					Logger.LogError("Can't save TutorialProgressPrcDto ({@dto}) for request: {request}", dto, message);

				await ProcessMessage(userId, value, message);
			}
		}
	}
}