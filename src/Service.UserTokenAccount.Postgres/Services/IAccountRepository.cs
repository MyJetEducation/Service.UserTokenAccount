namespace Service.UserTokenAccount.Postgres.Services
{
	public interface IAccountRepository
	{
		ValueTask<decimal> GetValueAsync(string userId);
		ValueTask<decimal?> UpdateValueAsync(string userId);
	}
}