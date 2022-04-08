namespace Service.UserTokenAccount.Postgres.Models
{
    public class UserTokenAccountEntity
    {
	    public UserTokenAccountEntity(string userId) => UserId = userId;

        public string UserId { get; set; }

        public DateTime? Date { get; set; }

        public decimal Value { get; set; }
    }
}
