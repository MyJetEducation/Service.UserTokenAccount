using Autofac;
using Service.Core.Client.Services;
using Service.UserTokenAccount.Postgres.Services;

namespace Service.UserTokenAccount.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
	        builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();
	        builder.RegisterType<OperationRepository>().AsImplementedInterfaces().SingleInstance();
	        builder.RegisterType<AccountRepository>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
