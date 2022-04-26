using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Postgres.Models;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Settings;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Service.UserTokenAccount.Jobs
{
	public abstract class NotificatorBase<TBusModel> where TBusModel : class
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IOperationRepository _operationRepository;
		protected readonly ISystemClock SystemClock;
		protected readonly ILogger Logger;

		protected NotificatorBase(IAccountRepository accountRepository, IOperationRepository operationRepository, ILogger logger, ISystemClock systemClock)
		{
			_accountRepository = accountRepository;
			_operationRepository = operationRepository;
			Logger = logger;
			SystemClock = systemClock;
		}

		protected async ValueTask ProcessMessage(string userId, decimal value, object message)
		{
			Logger.LogInformation("{model} handled from service bus for user: {user}", typeof (TBusModel), userId);

			if (value == 0m)
			{
				Logger.LogError("Can't calculate token increase value for request: {@request}", message);
				return;
			}

			bool increased = await _operationRepository.NewEntityAsync(new UserTokenOperationEntity
			{
				Value = value,
				UserId = userId,
				Movement = TokenOperationMovement.Income,
				Date = SystemClock.Now,
				Source = TokenOperationSource.TokenPurchase,
				Info = JsonSerializer.Serialize(message)
			});

			if (increased)
				await _accountRepository.UpdateValueAsync(userId);
			else
				Logger.LogError("Can't increase token account value for request: {@request}", message);
		}

		protected Func<TokenIncreaseValues> GetSettings() => Program.ReloadedSettings(model => model.TokenIncreaseValues);
	}
}