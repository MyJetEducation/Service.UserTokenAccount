using System.Runtime.Serialization;
using Service.MarketProduct.Domain.Models;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class GetOperationsGrpcRequest
	{
		[DataMember(Order = 1)]
		public string UserId { get; set; }

		[DataMember(Order = 2)]
		public TokenOperationMovement? Movement { get; set; }

		[DataMember(Order = 3)]
		public TokenOperationSource? Source { get; set; }

		[DataMember(Order = 4)]
		public MarketProductType? ProductType { get; set; }
	}
}