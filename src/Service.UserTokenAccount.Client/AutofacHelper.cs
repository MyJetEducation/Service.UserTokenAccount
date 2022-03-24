using Autofac;
using Microsoft.Extensions.Logging;
using Service.UserTokenAccount.Grpc;
using Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.UserTokenAccount.Client
{
    public static class AutofacHelper
    {
        public static void RegisterUserTokenAccountClient(this ContainerBuilder builder, string grpcServiceUrl, ILogger logger)
        {
            var factory = new UserTokenAccountClientFactory(grpcServiceUrl, logger);

            builder.RegisterInstance(factory.GetUserTokenAccountService()).As<IGrpcServiceProxy<IUserTokenAccountService>>().SingleInstance();
        }
    }
}
