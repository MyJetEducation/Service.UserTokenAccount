using System.Runtime.Serialization;

namespace Service.UserTokenAccount.Grpc.Models
{
	[DataContract]
	public class OperationsGrpcResponse
	{
		[DataMember(Order = 1)]
		public OperationGrpcModel[] Operations { get; set; }
	}
}