using System.Runtime.Serialization;
using Service.UserTokenAccount.Domain.Models;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class NewOperationGrpcResponse
	{
		[DataMember(Order = 1)]
		public decimal? Value { get; set; }

		[DataMember(Order = 2)]
		public TokenOperationResult Result { get; set; }

		public static NewOperationGrpcResponse Error(TokenOperationResult result) => new()
		{
			Result = result
		};
	}
}