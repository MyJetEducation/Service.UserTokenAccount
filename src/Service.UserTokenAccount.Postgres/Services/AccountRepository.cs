using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Postgres.Models;

namespace Service.UserTokenAccount.Postgres.Services
{
	public class AccountRepository : IAccountRepository
	{
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
		private readonly ILogger<AccountRepository> _logger;
		private readonly IOperationRepository _operationRepository;
		private readonly ISystemClock _systemClock;

		public AccountRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<AccountRepository> logger, IOperationRepository operationRepository, ISystemClock systemClock)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
			_logger = logger;
			_operationRepository = operationRepository;
			_systemClock = systemClock;
		}

		public async ValueTask<decimal> GetValueAsync(Guid? userId)
		{
			UserTokenAccountEntity entity = await GetEntity(userId);

			return (entity?.Value).GetValueOrDefault();
		}

		private async ValueTask<UserTokenAccountEntity> GetEntity(Guid? userId)
		{
			UserTokenAccountEntity accountEntity = null;

			try
			{
				accountEntity = await GetContext()
					.UserTokenAccountEntities
					.FirstOrDefaultAsync(entity => entity.UserId == userId);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);
			}

			return accountEntity;
		}

		public async ValueTask<decimal?> UpdateValueAsync(Guid? userId)
		{
			decimal? value = await _operationRepository.CountTotalAsync(userId);
			if (value == null)
				return await ValueTask.FromResult<decimal?>(null);

			DatabaseContext context = GetContext();
			DbSet<UserTokenAccountEntity> entities = context.UserTokenAccountEntities;
			UserTokenAccountEntity existingEntity = await GetEntity(userId);

			void FillEntity(UserTokenAccountEntity entity)
			{
				entity.Value = value.GetValueOrDefault();
				entity.Date = _systemClock.Now;
			}

			try
			{
				if (existingEntity == null)
				{
					var newEntity = new UserTokenAccountEntity(userId);
					FillEntity(newEntity);
					await entities.AddAsync(newEntity);
				}
				else
				{
					FillEntity(existingEntity);
					entities.Update(existingEntity);
				}

				await context.SaveChangesAsync();

				return value;
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);
			}

			return await ValueTask.FromResult<decimal?>(null);
		}

		private DatabaseContext GetContext() => DatabaseContext.Create(_dbContextOptionsBuilder);
	}
}