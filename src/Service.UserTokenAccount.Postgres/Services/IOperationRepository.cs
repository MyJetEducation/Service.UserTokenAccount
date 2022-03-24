using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Postgres.Models;

namespace Service.UserTokenAccount.Postgres.Services
{
	public interface IOperationRepository
	{
		ValueTask<UserTokenOperationEntity[]> GetEntitiesAsync(Guid? userId, TokenOperationMovement? movement, TokenOperationSource? source, MarketProductType? productType);

		ValueTask<bool> NewEntityAsync(UserTokenOperationEntity entity);
		
		ValueTask<decimal?> CountTotalAsync(Guid? userId);
	}
}