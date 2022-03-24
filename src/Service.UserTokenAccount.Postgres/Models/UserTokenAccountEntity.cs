namespace Service.UserTokenAccount.Postgres.Models
{
    public class UserTokenAccountEntity
    {
	    public UserTokenAccountEntity(Guid? userId) => UserId = userId;

        public Guid? UserId { get; set; }

        public DateTime? Date { get; set; }

        public decimal Value { get; set; }
    }
}
