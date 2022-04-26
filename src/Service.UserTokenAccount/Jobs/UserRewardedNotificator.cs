using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Constants;
using Service.Core.Client.Extensions;
using Service.Core.Client.Services;
using Service.ServiceBus.Models;
using Service.UserReward.Domain;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Settings;

namespace Service.UserTokenAccount.Jobs
{
	/// <summary>
	///     јчивки по типам, статусы
	/// </summary>
	public class UserRewardedNotificator : NotificatorBase<UserRewardedNotificator>
	{
		public UserRewardedNotificator(ILogger<UserRewardedNotificator> logger,
			ISubscriber<IReadOnlyList<UserRewardedServiceBusModel>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock) :
				base(accountRepository, operationRepository, logger, systemClock) => subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<UserRewardedServiceBusModel> events)
		{
			TokenIncreaseValues settings = GetSettings().Invoke();

			foreach (UserRewardedServiceBusModel message in events)
			{
				Logger.LogDebug("UserRewardedServiceBusModel handled from service bus: {@message}", message);

				var values = new List<decimal>();

				UserAchievement[] achievements = message.Achievements;
				if (!achievements.IsNullOrEmpty())
					values.AddRange(achievements.Select(achievement => GetAchievementIncreaseValue(achievement, settings)));

				UserStatusGrpcModel[] statuses = message.Statuses;
				if (!statuses.IsNullOrEmpty())
					values.AddRange(statuses.Select(model => GetStatusIncreaseValue(model.Status, settings)));

				decimal value = values.Sum();
				if (value == 0m)
					continue;

				await ProcessMessage(message.UserId, value, message);
			}
		}

		private decimal GetAchievementIncreaseValue(UserAchievement achievement, TokenIncreaseValues settings)
		{
			AchievementType achievementType = AchievementTypeHelper.GetAchievementType(achievement);

			switch (achievementType)
			{
				case AchievementType.Standard:
					return settings.AchievementStandard;
				case AchievementType.Rare:
					return settings.AchievementRare;
				case AchievementType.SuperRare:
					return settings.AchievementSuperRare;
				case AchievementType.UltraRare:
					return settings.AchievementUltraRare;
				case AchievementType.Unique:
					return settings.AchievementUnique;
				default:
					Logger.LogError("Can't get token increase setting for achievement type: {achievementType}", achievement);
					return 0m;
			}
		}

		private decimal GetStatusIncreaseValue(UserStatus status, TokenIncreaseValues settings)
		{
			switch (status)
			{
				case UserStatus.Analyst:
					return settings.StatusAnalyst;
				case UserStatus.Bachelor:
					return settings.StatusBachelor;
				case UserStatus.BestFriend:
					return settings.StatusBestFriend;
				case UserStatus.Expert:
					return settings.StatusExpert;
				case UserStatus.Financier:
					return settings.StatusFinancier;
				case UserStatus.Investor:
					return settings.StatusInvestor;
				case UserStatus.KeyMaster:
					return settings.StatusKeyMaster;
				case UserStatus.LongLiver:
					return settings.StatusLongLiver;
				case UserStatus.Magister:
					return settings.StatusMagister;
				case UserStatus.MasterOfOpenness:
					return settings.StatusMasterOfOpenness;
				case UserStatus.Newbie:
					return settings.StatusNewbie;
				case UserStatus.SecondYearStudent:
					return settings.StatusSecondYearStudent;
				case UserStatus.Strategist:
					return settings.StatusStrategist;
				case UserStatus.Rewarded:
					return settings.StatusRewarded;
				default:
					Logger.LogError("Can't get token increase setting for status: {status}", status);
					return 0m;
			}
		}
	}
}