using System.Collections.Generic;
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
		private readonly ITutorialProgressPrcRepository _tutorialProgressPrcRepository;

		public SetProgressInfoNotificator(ILogger<SetProgressInfoNotificator> logger,
			ISubscriber<IReadOnlyList<SetProgressInfoServiceBusModel>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock,
			ITutorialProgressPrcRepository tutorialProgressPrcRepository) :
				base(accountRepository, operationRepository, logger, systemClock)
		{
			_tutorialProgressPrcRepository = tutorialProgressPrcRepository;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<SetProgressInfoServiceBusModel> events)
		{
			TokenIncreaseValues settings = GetSettings().Invoke();

			foreach (SetProgressInfoServiceBusModel message in events)
			{
				int tutorialProgress = message.TutorialProgress;

				if (!message.SetUserProgress || !tutorialProgress.IsOkProgress())
					continue;

				string userId = message.UserId;

				TutorialProgressPrcDto prcInfo = await _tutorialProgressPrcRepository.Get(userId, message.Tutorial);
				if (prcInfo.SetOkPrc && prcInfo.SetMaxPrc)
					continue;

				var value = 0m;

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

				if (value == 0m)
					continue;

				bool flagsSettet = await _tutorialProgressPrcRepository.Save(userId, prcInfo);
				if (!flagsSettet)
					Logger.LogError("Can't save TutorialProgressPrcDto ({dto}) for request: {request}", prcInfo, message);

				await ProcessMessage(userId, value, message);
			}
		}
	}
}