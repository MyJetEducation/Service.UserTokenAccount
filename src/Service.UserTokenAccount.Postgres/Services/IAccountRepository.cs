namespace Service.UserTokenAccount.Postgres.Services
{
	public interface IAccountRepository
	{
		ValueTask<decimal> GetValueAsync(Guid? userId);
		ValueTask<decimal?> UpdateValueAsync(Guid? userId);
	}
}