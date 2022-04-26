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
	///     Первый логин (регистрация), ежедневное начисление при логине
	/// </summary>
	public class ClientLoginInfoNotificator : NotificatorBase<ClientLoginInfoNotificator>
	{
		private readonly string _settingsKey = Program.Settings.KeyUserLoginInfo;

		private readonly IServerKeyValueDtoRepository<UserLoginInfoDto> _serverKeyValueDtoRepository;

		public ClientLoginInfoNotificator(ILogger<ClientLoginInfoNotificator> logger,
			ISubscriber<IReadOnlyList<ClientAuditLogModel>> subscriber,
			IAccountRepository accountRepository,
			IOperationRepository operationRepository,
			ISystemClock systemClock, IServerKeyValueDtoRepository<UserLoginInfoDto> serverKeyValueDtoRepository) :
				base(accountRepository, operationRepository, logger, systemClock)
		{
			_serverKeyValueDtoRepository = serverKeyValueDtoRepository;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<ClientAuditLogModel> events)
		{
			DateTime nowDate = SystemClock.Now;

			foreach (ClientAuditLogModel message in events)
			{
				string userId = message.ClientId;

				UserLoginInfoDto dto = await _serverKeyValueDtoRepository.Get(_settingsKey, userId);

				int value;
				if (dto == null)
				{
					dto = new UserLoginInfoDto {LastLoginDate = nowDate};
					value = GetSettings().Invoke().Register;
				}
				else if (dto.LastLoginDate.Date != nowDate.Date)
				{
					dto.LastLoginDate = nowDate;
					value = GetSettings().Invoke().Daily;
				}
				else continue;

				bool dtoSaved = await _serverKeyValueDtoRepository.Save(_settingsKey, userId, dto);
				if (!dtoSaved)
					Logger.LogError("Can't save UserLoginInfoDto ({@dto}) for request: {request}", dto, message);

				await ProcessMessage(userId, value, message);
			}
		}
	}
}