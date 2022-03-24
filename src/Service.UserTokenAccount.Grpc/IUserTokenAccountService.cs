using System.ServiceModel;
using System.Threading.Tasks;
using Service.UserTokenAccount.Grpc.Models;

namespace Service.UserTokenAccount.Grpc
{
	[ServiceContract]
	public interface IUserTokenAccountService
	{
		[OperationContract]
		ValueTask<OperationsGrpcResponse> GetOperationsAsync(GetOperationsGrpcRequest request);

		[OperationContract]
		ValueTask<NewOperationGrpcResponse> NewOperationAsync(NewOperationGrpcRequest request);

		[OperationContract]
		ValueTask<AccountGrpcResponse> GetAccountAsync(GetAccountGrpcRequest request);
	}
}