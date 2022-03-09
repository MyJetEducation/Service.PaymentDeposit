using Service.PaymentDeposit.Grpc.Models;
using Service.PaymentDeposit.Models;

namespace Service.PaymentDeposit.Services
{
	public class PaymentProviderRouter : IPaymentProviderRouter
	{
		public PaymentProviderBridgeInfo GetPaymentProviderBridge(DepositGrpcRequest request) => new PaymentProviderBridgeInfo
		{
			Name = "test",
			Url = Program.Settings.PaymentProviderBridgeTestServiceUrl
		};
	}
}