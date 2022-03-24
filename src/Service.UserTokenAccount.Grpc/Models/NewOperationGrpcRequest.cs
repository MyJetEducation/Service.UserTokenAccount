using System;
using System.Runtime.Serialization;
using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class NewOperationGrpcRequest
	{
		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }

		[DataMember(Order = 2)]
		public TokenOperationMovement Movement { get; set; }

		[DataMember(Order = 3)]
		public TokenOperationSource Source { get; set; }

		[DataMember(Order = 4)]
		public MarketProductType? ProductType { get; set; }

		[DataMember(Order = 5)]
		public decimal Value { get; set; }

		[DataMember(Order = 6)]
		public string Info { get; set; }
	}
}