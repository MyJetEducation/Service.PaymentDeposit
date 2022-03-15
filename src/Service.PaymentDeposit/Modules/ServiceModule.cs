using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.TcpClient;
using Service.PaymentDeposit.Services;
using Service.PaymentDepositRepository.Client;
using Service.PaymentProviderRouter.Client;
using Service.ServiceBus.Models;
using Service.UserPaymentCard.Client;

namespace Service.PaymentDeposit.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterPaymentDepositRepositoryClient(Program.Settings.PaymentDepositRepositoryServiceUrl, Program.LogFactory.CreateLogger(typeof (PaymentDepositRepositoryClientFactory)));
			builder.RegisterPaymentProviderRouterClient(Program.Settings.PaymentProviderRouterServiceUrl, Program.LogFactory.CreateLogger(typeof(PaymentProviderRouterClientFactory)));
			builder.RegisterUserPaymentCardClient(Program.Settings.UserPaymentCardServiceUrl, Program.LogFactory.CreateLogger(typeof(UserPaymentCardClientFactory)));

			builder.RegisterType<PaymentProviderResolver>().AsImplementedInterfaces().SingleInstance();

			var tcpServiceBus = new MyServiceBusTcpClient(() => Program.Settings.ServiceBusWriter, "MyJetEducation Service.PaymentDeposit");

			builder
				.Register(context => new MyServiceBusPublisher<NewPaymentServiceBusModel>(tcpServiceBus, NewPaymentServiceBusModel.TopicName, false))
				.As<IServiceBusPublisher<NewPaymentServiceBusModel>>()
				.SingleInstance();

			tcpServiceBus.Start();
		}
	}
}