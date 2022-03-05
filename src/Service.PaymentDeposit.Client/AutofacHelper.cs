using Autofac;
using Microsoft.Extensions.Logging;
using Service.PaymentDeposit.Grpc;
using Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.PaymentDeposit.Client
{
    public static class AutofacHelper
    {
        public static void RegisterPaymentDepositClient(this ContainerBuilder builder, string grpcServiceUrl, ILogger logger)
        {
            var factory = new PaymentDepositClientFactory(grpcServiceUrl, logger);

            builder.RegisterInstance(factory.GetPaymentDepositService()).As<IGrpcServiceProxy<IPaymentDepositService>>().SingleInstance();
        }
    }
}
