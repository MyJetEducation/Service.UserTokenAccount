using System.Runtime.Serialization;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class AccountGrpcResponse
	{
		[DataMember(Order = 1)]
		public decimal Value { get; set; }
	}
}