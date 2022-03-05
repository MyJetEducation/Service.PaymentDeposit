using Service.PaymentDeposit.Grpc.Models;
using Service.PaymentDeposit.Models;

namespace Service.PaymentDeposit.Services
{
	public interface IPaymentProviderResolver
	{
		PaymentProviderBridgeInfo GetPaymentProviderBridge(DepositGrpcRequest request);
	}
}