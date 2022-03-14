using Autofac;
using Microsoft.Extensions.Logging;
using Service.PaymentDeposit.Services;
using Service.PaymentDepositRepository.Client;
using Service.PaymentProviderRouter.Client;

namespace Service.PaymentDeposit.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterPaymentDepositRepositoryClient(Program.Settings.PaymentDepositRepositoryServiceUrl, Program.LogFactory.CreateLogger(typeof (PaymentDepositRepositoryClientFactory)));
			builder.RegisterPaymentProviderRouterClient(Program.Settings.PaymentProviderRouterServiceUrl, Program.LogFactory.CreateLogger(typeof(PaymentProviderRouterClientFactory)));

			builder.RegisterType<PaymentProviderResolver>().AsImplementedInterfaces().SingleInstance();
		}
	}
}