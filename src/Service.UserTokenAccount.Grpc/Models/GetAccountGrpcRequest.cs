using System.Runtime.Serialization;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class GetAccountGrpcRequest
	{
		[DataMember(Order = 1)]
		public string UserId { get; set; }
	}
}