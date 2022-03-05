using System.ServiceModel;
using System.Threading.Tasks;
using Service.PaymentDeposit.Grpc.Models;

namespace Service.PaymentDeposit.Grpc
{
	[ServiceContract]
	public interface IPaymentDepositService
	{
		[OperationContract]
		ValueTask<DepositGrpcResponse> DepositAsync(DepositGrpcRequest request);

		[OperationContract]
		ValueTask CallbackAsync(CallbackGrpcRequest request);
	}
}