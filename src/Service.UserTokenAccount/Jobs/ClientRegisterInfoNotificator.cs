using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Services;
using Service.Registration.Domain.Models;
using Service.UserTokenAccount.Postgres.Services;

namespace Service.UserTokenAccount.Jobs
{
	/// <summary>
	///     Первый логин (регистрация)
	/// </summary>
	public class ClientRegisterInfoNotificator : NotificatorBase<ClientRegisterInfoNotificator>
	{
		public ClientRegisterInfoNotificator(ILogger<ClientRegisterInfoNotificator> logger,
			ISubscriber<IReadOnlyList<ClientRegisterMessage>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock) :
				base(accountRepository, operationRepository, logger, systemClock) => subscriber.Subscribe(HandleEvent);

		private async ValueTask HandleEvent(IReadOnlyList<ClientRegisterMessage> events)
		{
			int value = GetSettings().Invoke().Register;

			foreach (ClientRegisterMessage message in events)
				await ProcessMessage(message.TraderId, value, message);
		}
	}
}