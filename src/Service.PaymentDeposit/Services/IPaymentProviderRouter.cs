using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDeposit.Models;

namespace Service.PaymentDeposit.Services
{
	public interface IPaymentProviderRouter
	{
		IPaymentProviderGrpcService GetProviderBridge(PaymentProviderBridgeInfo bridgeInfo);
	}
}