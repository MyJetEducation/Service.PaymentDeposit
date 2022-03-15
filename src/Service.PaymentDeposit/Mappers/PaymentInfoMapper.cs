using System;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDeposit.Models;
using Service.PaymentDepositRepository.Grpc.Models;
using Service.PaymentProviderRouter.Grpc.Models;
using Service.ServiceBus.Models;

namespace Service.PaymentDeposit.Mappers
{
	public static class PaymentInfoMapper
	{
		public static ProviderDepositGrpcRequest ToBridgeGrpcModel(this PaymentInfo info, Guid? transactionId)
		{
			PaymentCardInfo cardInfo = info.CardInfo;

			return new ProviderDepositGrpcRequest
			{
				UserId = info.UserId,
				Amount = info.Amount,
				Currency = info.Currency,
				Country = info.Country,
				Number = cardInfo.Number,
				Holder = cardInfo.Holder,
				Month = cardInfo.Month,
				Year = cardInfo.Year,
				Cvv = cardInfo.Cvv,
				TransactionId = transactionId
			};
		}

		public static GetPaymentProviderBridgeGrpcRequest ToRouterGrpcModel(this PaymentInfo info) => new GetPaymentProviderBridgeGrpcRequest
		{
			UserId = info.UserId,
			Amount = info.Amount,
			Currency = info.Currency,
			Country = info.Country,
			ServiceCode = info.ServiceCode
		};

		public static RegisterGrpcRequest ToGrpcModel(this PaymentInfo info, PaymentProviderBridgeInfo paymentProvider) => new RegisterGrpcRequest
		{
			UserId = info.UserId,
			Amount = info.Amount,
			Provider = paymentProvider.ProviderCode,
			Currency = info.Currency,
			Country = info.Country,
			ServiceCode = info.ServiceCode,
			CardId = info.CardInfo.CardId
		};

		public static NewPaymentServiceBusModel ToNewPaymentServiceBusModel(this PaymentInfo info, Guid? transactionId) => new NewPaymentServiceBusModel
		{
			UserId = info.UserId,
			CardId = info.CardInfo.CardId,
			TransactionId = transactionId
		};
	}
}