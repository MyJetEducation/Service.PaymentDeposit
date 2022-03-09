using System.ServiceModel;
using System.Threading.Tasks;

namespace Service.PaymentDeposit.Domain.Models
{
	[ServiceContract]
	public interface IPaymentProviderGrpcService
	{
		[OperationContract]
		ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request);
	}
}