using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Postgres.Models;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Settings;

namespace Service.UserTokenAccount.Jobs
{
	public abstract class NotificatorBase<TBusModel> where TBusModel : class
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IOperationRepository _operationRepository;
		private readonly ISystemClock _systemClock;
		protected readonly ILogger Logger;

		protected NotificatorBase(IAccountRepository accountRepository, IOperationRepository operationRepository, ILogger logger, ISystemClock systemClock)
		{
			_accountRepository = accountRepository;
			_operationRepository = operationRepository;
			Logger = logger;
			_systemClock = systemClock;
		}

		protected async ValueTask ProcessMessage(string userId, decimal value, object message)
		{
			Logger.LogInformation("{model} handled from service bus: {user}", typeof (TBusModel), userId);

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
				Date = _systemClock.Now,
				Source = TokenOperationSource.TokenPurchase,
				Info = JsonConvert.SerializeObject(message)
			});

			if (increased)
				await _accountRepository.UpdateValueAsync(userId);
			else
				Logger.LogError("Can't increase token account value for request: {@request}", message);
		}

		protected Func<TokenIncreaseValues> GetSettings() => Program.ReloadedSettings(model => model.TokenIncreaseValues);
	}
}