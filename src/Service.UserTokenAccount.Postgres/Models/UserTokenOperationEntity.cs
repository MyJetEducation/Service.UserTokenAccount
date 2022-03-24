using System.ComponentModel.DataAnnotations.Schema;
using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Postgres.Models
{
	public class UserTokenOperationEntity
	{
		public int? Id { get; set; }

		public Guid? UserId { get; set; }

		public DateTime? Date { get; set; }

		public TokenOperationMovement Movement { get; set; }

		public TokenOperationSource Source { get; set; }

		public MarketProductType? ProductType { get; set; }

		public decimal Value { get; set; }

		[Column(TypeName = "jsonb")]
		public string Info { get; set; }
	}
}