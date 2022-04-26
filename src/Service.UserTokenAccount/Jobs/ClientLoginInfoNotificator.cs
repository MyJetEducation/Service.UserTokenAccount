using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.ClientAuditLog.Domain.Models;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Services;

namespace Service.UserTokenAccount.Jobs
{
	/// <summary>
	///     Первый логин (регистрация)
	/// </summary>
	public class ClientLoginInfoNotificator : NotificatorBase<ClientLoginInfoNotificator>
	{
		private readonly IUserFirstLoginRepository _userFirstLoginRepository;

		public ClientLoginInfoNotificator(ILogger<ClientLoginInfoNotificator> logger,
			ISubscriber<IReadOnlyList<ClientAuditLogModel>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock, IUserFirstLoginRepository userFirstLoginRepository) :
				base(accountRepository, operationRepository, logger, systemClock)
		{
			_userFirstLoginRepository = userFirstLoginRepository;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<ClientAuditLogModel> events)
		{
			int value = GetSettings().Invoke().Register;

			foreach (ClientAuditLogModel message in events)
			{
				string userId = message.ClientId;

				if (await _userFirstLoginRepository.WasLogin(userId))
					continue;

				await ProcessMessage(userId, value, message);
			}
		}
	}
}