using System;
using System.Runtime.Serialization;
using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class OperationGrpcModel
	{
		[DataMember(Order = 1)]
		public string UserId { get; set; }

		[DataMember(Order = 2)]
		public DateTime? Date { get; set; }

		[DataMember(Order = 3)]
		public TokenOperationMovement Movement { get; set; }

		[DataMember(Order = 4)]
		public TokenOperationSource Source { get; set; }

		[DataMember(Order = 5)]
		public MarketProductType? ProductType { get; set; }

		[DataMember(Order = 6)]
		public decimal Value { get; set; }

		[DataMember(Order = 7)]
		public string Info { get; set; }
	}
}