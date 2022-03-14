using Service.PaymentDeposit.Domain.Models;
using Service.PaymentProviderRouter.Grpc.Models;

namespace Service.PaymentDeposit.Services
{
	public interface IPaymentProviderResolver
	{
		IPaymentProviderGrpcService GetProviderBridge(PaymentProviderBridgeInfo bridgeInfo);
	}
}