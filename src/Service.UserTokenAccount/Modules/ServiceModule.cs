using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.ClientAuditLog.Domain.Models;
using Service.Core.Client.Services;
using Service.ServerKeyValue.Client;
using Service.ServiceBus.Models;
using Service.UserTokenAccount.Domain.Models;
using Service.UserTokenAccount.Jobs;
using Service.UserTokenAccount.Postgres.Services;
using Service.UserTokenAccount.Services;

namespace Service.UserTokenAccount.Modules
{
	public class ServiceModule : Module
	{
		private const string QueueName = "MyJetEducation-UserTokenAccount";

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof(ServerKeyValueClientFactory)));

			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<OperationRepository>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<AccountRepository>().AsImplementedInterfaces().SingleInstance();

			builder.RegisterType<ServerKeyValueDtoRepository<TutorialProgressPrcDto[]>>().As<IServerKeyValueDtoRepository<TutorialProgressPrcDto[]>>().SingleInstance();
			builder.RegisterType<ServerKeyValueDtoRepository<UserFirstLoginDto>>().As<IServerKeyValueDtoRepository<UserFirstLoginDto>>().SingleInstance();
			builder.RegisterType<ServerKeyValueDtoRepository<UserLastDailyTokenIncreaseDto>>().As<IServerKeyValueDtoRepository<UserLastDailyTokenIncreaseDto>>().SingleInstance();

			builder.RegisterType<SetProgressInfoNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<ClientLoginInfoNotificator>().AutoActivate().SingleInstance();
			builder.RegisterType<UserRewardedNotificator>().AutoActivate().SingleInstance();

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterMyServiceBusSubscriberBatch<SetProgressInfoServiceBusModel>(serviceBusClient, SetProgressInfoServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<ClientAuditLogModel>(serviceBusClient, ClientAuditLogModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<UserRewardedServiceBusModel>(serviceBusClient, UserRewardedServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
		}
	}
}