using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Service.UserTokenAccount.Grpc;
using Service.Grpc;

namespace Service.UserTokenAccount.Client
{
    [UsedImplicitly]
    public class UserTokenAccountClientFactory : GrpcClientFactory
    {
        public UserTokenAccountClientFactory(string grpcServiceUrl, ILogger logger) : base(grpcServiceUrl, logger)
        {
        }

        public IGrpcServiceProxy<IUserTokenAccountService> GetUserTokenAccountService() => CreateGrpcService<IUserTokenAccountService>();
    }
}
