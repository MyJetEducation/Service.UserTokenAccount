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
using Service.UserTokenAccount.Settings;

namespace Service.UserTokenAccount.Jobs
{
	/// <summary>
	///     Завершение дисциплины (80%, 100%)
	/// </summary>
	public class SetProgressInfoNotificator : NotificatorBase<SetProgressInfoNotificator>
	{
		private readonly Func<string> _tutorialProgressPrcKey = Program.ReloadedSettings(model => model.KeyTutorialProgressPrc);

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
			TokenIncreaseValues settings = GetSettings().Invoke();
			string settingsKey = _tutorialProgressPrcKey.Invoke();

			foreach (SetProgressInfoServiceBusModel message in events)
			{
				int tutorialProgress = message.TutorialProgress;

				if (!message.SetUserProgress || !tutorialProgress.IsOkProgress())
					continue;

				string userId = message.UserId;

				List<TutorialProgressPrcDto> dtos = (await _serverKeyValueDtoRepository.Get(settingsKey, userId) ?? Array.Empty<TutorialProgressPrcDto>()).ToList();
				TutorialProgressPrcDto prcInfo = dtos.FirstOrDefault(dto => dto.Tutorial == message.Tutorial);
				if (prcInfo == null)
				{
					prcInfo = new TutorialProgressPrcDto(){Tutorial = message.Tutorial};
					dtos.Add(prcInfo);
				}

				if (prcInfo.SetOkPrc && prcInfo.SetMaxPrc)
					continue;

				int value = 0;

				if (tutorialProgress.IsMaxProgress() && !prcInfo.SetMaxPrc)
				{
					value = settings.TutorialFinished100;
					prcInfo.SetMaxPrc = true;
				}

				if (tutorialProgress.IsOkProgress() && !prcInfo.SetOkPrc)
				{
					value = settings.TutorialFinished80;
					prcInfo.SetOkPrc = true;
				}

				if (value == 0)
					continue;

				bool flagsSettet = await _serverKeyValueDtoRepository.Save(settingsKey, userId, dtos.ToArray());
				if (!flagsSettet)
					Logger.LogError("Can't save TutorialProgressPrcDto ({dto}) for request: {request}", prcInfo, message);

				await ProcessMessage(userId, value, message);
			}
		}
	}
}