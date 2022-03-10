using Autofac;
using Microsoft.Extensions.Logging;
using Service.PaymentDeposit.Services;
using Service.PaymentDepositRepository.Client;

namespace Service.PaymentDeposit.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterPaymentDepositRepositoryClient(Program.Settings.PaymentDepositRepositoryServiceUrl, Program.LogFactory.CreateLogger(typeof (PaymentDepositRepositoryClientFactory)));

			builder.RegisterType<PaymentProviderRouter>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<PaymentProviderRouter>().AsImplementedInterfaces().SingleInstance();
		}
	}
}