using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.ClientAuditLog.Domain.Models;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Services;

namespace Service.UserTokenAccount.Jobs
{
	/// <summary>
	///     Первый логин (регистрация)
	/// </summary>
	public class ClientLoginInfoNotificator : NotificatorBase<ClientLoginInfoNotificator>
	{
		private readonly Func<string> _firstLoginKey = Program.ReloadedSettings(model => model.KeyFirstLogin);

		private readonly IServerKeyValueDtoRepository<UserFirstLoginDto> _serverKeyValueDtoRepository;

		public ClientLoginInfoNotificator(ILogger<ClientLoginInfoNotificator> logger,
			ISubscriber<IReadOnlyList<ClientAuditLogModel>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock, IServerKeyValueDtoRepository<UserFirstLoginDto> serverKeyValueDtoRepository) :
				base(accountRepository, operationRepository, logger, systemClock)
		{
			_serverKeyValueDtoRepository = serverKeyValueDtoRepository;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<ClientAuditLogModel> events)
		{
			int value = GetSettings().Invoke().Register;

			foreach (ClientAuditLogModel message in events)
			{
				string userId = message.ClientId;
				string firstLoginKey = _firstLoginKey.Invoke();

				UserFirstLoginDto firstLoginDto = await _serverKeyValueDtoRepository.Get(firstLoginKey, userId);
				if (firstLoginDto?.WasLogin == true)
					continue;

				await ProcessMessage(userId, value, message);

				bool response = await _serverKeyValueDtoRepository.Save(firstLoginKey, userId, new UserFirstLoginDto {WasLogin = true});
				if (!response)
					Logger.LogError("Can't save user first login flag for request: {@request}", message);
			}
		}
	}
}