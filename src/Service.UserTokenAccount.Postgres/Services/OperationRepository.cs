using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Extensions;
using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Postgres.Models;

namespace Service.UserTokenAccount.Postgres.Services
{
	public class OperationRepository : IOperationRepository
	{
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
		private readonly ILogger<OperationRepository> _logger;

		public OperationRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ILogger<OperationRepository> logger)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
			_logger = logger;
		}

		public async ValueTask<UserTokenOperationEntity[]> GetEntitiesAsync(string userId, TokenOperationMovement? movement, TokenOperationSource? source, MarketProductType? productType)
		{
			try
			{
				return await GetContext()
					.UserTokenOperationEntities
					.WhereIf(userId != null, entity => entity.UserId == userId)
					.WhereIf(movement != null, entity => entity.Movement == movement)
					.WhereIf(source != null, entity => entity.Source == source)
					.WhereIf(productType != null, entity => entity.ProductType == productType)
					.ToArrayAsync();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);

				return Array.Empty<UserTokenOperationEntity>();
			}
		}

		public async ValueTask<bool> NewEntityAsync(UserTokenOperationEntity entity)
		{
			try
			{
				DatabaseContext context = GetContext();

				await context
					.UserTokenOperationEntities
					.AddAsync(entity);

				await context.SaveChangesAsync();

				return true;
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);

				return false;
			}
		}

		public async ValueTask<decimal?> CountTotalAsync(string userId)
		{
			try
			{
				var operationsData = await GetContext()
					.UserTokenOperationEntities
					.Where(entity => entity.UserId == userId)
					.Select(entity => new {entity.Value, entity.Movement})
					.ToArrayAsync();

				return operationsData.IsNullOrEmpty()
					? 0
					: operationsData.Sum(arg => arg.Value * GetMovementMultiplier(arg.Movement));
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);

				return await ValueTask.FromResult<decimal?>(null);
			}
		}

		private static int GetMovementMultiplier(TokenOperationMovement movement) =>
			movement switch {
				TokenOperationMovement.Income => 1,
				TokenOperationMovement.Outcome => -1,
				_ => throw new ArgumentException($"Can't use movement {movement} for calculate user token account value")
				};

		private DatabaseContext GetContext() => DatabaseContext.Create(_dbContextOptionsBuilder);
	}
}