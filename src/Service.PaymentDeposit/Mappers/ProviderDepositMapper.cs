using System;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDeposit.Grpc.Models;

namespace Service.PaymentDeposit.Mappers
{
	public static class ProviderDepositMapper
	{
		public static ProviderDepositGrpcRequest ToGrpcModel(this DepositGrpcRequest request, Guid? transactionId) => new ProviderDepositGrpcRequest
		{
			UserId = request.UserId,
			Amount = request.Amount,
			Currency = request.Currency,
			Country = request.Country,
			Number = request.Number,
			Holder = request.Holder,
			Month = request.Month,
			Year = request.Year,
			Cvv = request.Cvv,
			TransactionId = transactionId
		};
	}
}