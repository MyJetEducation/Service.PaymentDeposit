using System.Threading.Tasks;

namespace Service.PaymentDeposit.Domain.Models
{
	public interface IPaymentProviderGrpcService
	{
		ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request);
	}
}