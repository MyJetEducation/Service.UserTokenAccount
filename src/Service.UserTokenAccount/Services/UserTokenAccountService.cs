using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Grpc;
using Service.UserTokenAccount.Grpc.Models;
using Service.UserTokenAccount.Mappers;
using Service.UserTokenAccount.Postgres.Models;
using Service.UserTokenAccount.Postgres.Services;

namespace Service.UserTokenAccount.Services
{
	public class UserTokenAccountService : IUserTokenAccountService
	{
		private readonly ILogger<UserTokenAccountService> _logger;
		private readonly IOperationRepository _operationRepository;
		private readonly IAccountRepository _accountRepository;
		private readonly ISystemClock _systemClock;

		private static readonly SemaphoreSlim OperationLocker = new SemaphoreSlim(1, 1);

		public UserTokenAccountService(ILogger<UserTokenAccountService> logger, IOperationRepository operationRepository, IAccountRepository accountRepository, ISystemClock systemClock)
		{
			_logger = logger;
			_operationRepository = operationRepository;
			_accountRepository = accountRepository;
			_systemClock = systemClock;
		}

		public async ValueTask<OperationsGrpcResponse> GetOperationsAsync(GetOperationsGrpcRequest request)
		{
			UserTokenOperationEntity[] entities = await _operationRepository.GetEntitiesAsync(request.UserId, request.Movement, request.Source, request.ProductType);

			return new OperationsGrpcResponse
			{
				Operations = entities.Select(entity => entity.ToGrpcModel()).ToArray()
			};
		}

		public async ValueTask<NewOperationGrpcResponse> NewOperationAsync(NewOperationGrpcRequest request)
		{
			await OperationLocker.WaitAsync();

			string userId = request.UserId;

			try
			{
				if (!await ValidateRequest(request))
				{
					_logger.LogWarning("Insufficient account value for request: {@request}", request);

					return NewOperationGrpcResponse.Error(TokenOperationResult.InsufficientAccount);
				}

				bool newEntityResult = await _operationRepository.NewEntityAsync(request.ToModel(_systemClock.Now));
				if (!newEntityResult)
					NewOperationGrpcResponse.Error(TokenOperationResult.Failed);

				decimal? newAccountValue = await _accountRepository.UpdateValueAsync(userId);
				if (newAccountValue == null || newAccountValue < 0)
				{
					_logger.LogError("Invalid account value ({$newValue}) saved after new operation with user account for request {@request}", newAccountValue, request);

					return NewOperationGrpcResponse.Error(TokenOperationResult.Failed);
				}

				return new NewOperationGrpcResponse
				{
					Value = newAccountValue.Value,
					Result = TokenOperationResult.Ok
				};
			}
			finally
			{
				OperationLocker.Release();
			}
		}

		private async ValueTask<bool> ValidateRequest(NewOperationGrpcRequest request)
		{
			if (request.Movement != TokenOperationMovement.Outcome)
				return true;

			decimal accountValue = await GetAccountValue(request.UserId);

			return accountValue >= request.Value;
		}

		private ValueTask<decimal> GetAccountValue(string userId) => _accountRepository.GetValueAsync(userId);

		public async ValueTask<AccountGrpcResponse> GetAccountAsync(GetAccountGrpcRequest request) => new AccountGrpcResponse
		{
			Value = await _accountRepository.GetValueAsync(request.UserId)
		};
	}
}