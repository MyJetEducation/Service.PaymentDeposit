using System;
using Service.PaymentDepositRepository.Grpc.Models;
using Service.ServiceBus.Models;

namespace Service.PaymentDeposit.Mappers
{
	public static class DepositGrpcModelMapper
	{
		public static NewPaymentServiceBusModel ToNewPaymentServiceBusModel(this DepositGrpcModel deposit, Guid? cardId) => new NewPaymentServiceBusModel
		{
			UserId = deposit.UserId,
			TransactionId = deposit.TransactionId,
			CardId = cardId
		};
	}
}