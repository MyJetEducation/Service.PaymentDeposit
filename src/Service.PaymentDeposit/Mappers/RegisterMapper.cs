using Service.PaymentDeposit.Grpc.Models;
using Service.PaymentDeposit.Models;
using Service.PaymentDepositRepository.Grpc.Models;

namespace Service.PaymentDeposit.Mappers
{
	public static class RegisterMapper
	{
		public static RegisterGrpcRequest ToGrpcModel(this DepositGrpcRequest request, PaymentProviderBridgeInfo paymentProvider) => new RegisterGrpcRequest
		{
			UserId = request.UserId,
			Amount = request.Amount,
			Provider = paymentProvider.Name,
			Currency = request.Currency,
			Country = request.Country,
			ServiceCode = request.ServiceCode,
			Number = request.Number,
			Holder = request.Holder,
			Month = request.Month,
			Year = request.Year,
			Cvv = request.Cvv
		};
	}
}