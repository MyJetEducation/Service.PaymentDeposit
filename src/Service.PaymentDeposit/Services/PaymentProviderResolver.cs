using Service.PaymentDeposit.Grpc.Models;
using Service.PaymentDeposit.Models;

namespace Service.PaymentDeposit.Services
{
	public class PaymentProviderResolver : IPaymentProviderResolver
	{
		public PaymentProviderBridgeInfo GetPaymentProviderBridge(DepositGrpcRequest request) => new PaymentProviderBridgeInfo
		{
			Name = "test",
			Url = "http://"
		};
	}
}