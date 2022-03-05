using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Service.PaymentDeposit.Grpc;
using Service.Grpc;

namespace Service.PaymentDeposit.Client
{
    [UsedImplicitly]
    public class PaymentDepositClientFactory : GrpcClientFactory
    {
        public PaymentDepositClientFactory(string grpcServiceUrl, ILogger logger) : base(grpcServiceUrl, logger)
        {
        }

        public IGrpcServiceProxy<IPaymentDepositService> GetPaymentDepositService() => CreateGrpcService<IPaymentDepositService>();
    }
}
