using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDeposit.Grpc;
using Service.PaymentDeposit.Grpc.Models;
using Service.PaymentDeposit.Mappers;
using Service.PaymentDeposit.Models;
using Service.PaymentDepositRepository.Domain.Models;
using Service.PaymentDepositRepository.Grpc;
using Service.PaymentDepositRepository.Grpc.Models;
using Service.PaymentProviderRouter.Grpc;
using Service.PaymentProviderRouter.Grpc.Models;
using Service.ServiceBus.Models;
using Service.UserPaymentCard.Grpc;
using Service.UserPaymentCard.Grpc.Models;
using DepositGrpcResponse = Service.PaymentDeposit.Grpc.Models.DepositGrpcResponse;

namespace Service.PaymentDeposit.Services
{
	public class PaymentDepositService : IPaymentDepositService
	{
		private readonly ILogger<PaymentDepositService> _logger;
		private readonly IGrpcServiceProxy<IPaymentProviderRouterService> _paymentProviderRouter;
		private readonly IPaymentProviderResolver _paymentProviderResolver;
		private readonly IGrpcServiceProxy<IPaymentDepositRepositoryService> _paymentDepositRepositoryService;
		private readonly IGrpcServiceProxy<IUserPaymentCardService> _userPaymentCardService;
		private readonly IServiceBusPublisher<NewPaymentServiceBusModel> _newPaymentPublisher;

		public PaymentDepositService(ILogger<PaymentDepositService> logger,
			IGrpcServiceProxy<IPaymentProviderRouterService> paymentProviderRouter,
			IPaymentProviderResolver paymentProviderResolver,
			IGrpcServiceProxy<IPaymentDepositRepositoryService> paymentDepositRepositoryService, 
			IGrpcServiceProxy<IUserPaymentCardService> userPaymentCardService, 
			IServiceBusPublisher<NewPaymentServiceBusModel> newPaymentPublisher)
		{
			_logger = logger;
			_paymentProviderRouter = paymentProviderRouter;
			_paymentProviderResolver = paymentProviderResolver;
			_paymentDepositRepositoryService = paymentDepositRepositoryService;
			_userPaymentCardService = userPaymentCardService;
			_newPaymentPublisher = newPaymentPublisher;
		}

		public async ValueTask<DepositGrpcResponse> DepositAsync(DepositGrpcRequest request)
		{
			PaymentInfo info = await CreatePaymentInfo(request);

			PaymentProviderBridgeInfo bridgeInfo = await _paymentProviderRouter.Service.GetPaymentProviderBridgeAsync(info.ToRouterGrpcModel());
			if (bridgeInfo?.ProviderCode == null)
				return GetErrorResponse("Can't find deposit provider for payment info: {@info}", info);

			_logger.LogDebug("PaymentProviderBridgeInfo recieved: {@info}", bridgeInfo);

			IPaymentProviderGrpcService bridge = _paymentProviderResolver.GetProviderBridge(bridgeInfo);
			if (bridge == null)
				return GetErrorResponse("Can't create provider bridge for payment info: {@info}, bridge info: {@bridgeInfo}", info, bridgeInfo);

			_logger.LogDebug("PaymentProviderGrpcService connection created: {@bridge}", bridge);

			RegisterGrpcResponse registerGrpcResponse = await _paymentDepositRepositoryService.TryCall(service => service.RegisterAsync(info.ToGrpcModel(bridgeInfo)));
			if (registerGrpcResponse == null)
				return GetErrorResponse("Can't register deposit transaction for payment info: {@info}, bridge info: {@bridgeInfo}", info, bridgeInfo);

			Guid? transactionId = registerGrpcResponse.TransactionId;
			_logger.LogDebug("Transaction registered with id: {id}", transactionId);

			ProviderDepositGrpcRequest providerDepositGrpcRequest = info.ToBridgeGrpcModel(transactionId);
			ProviderDepositGrpcResponse depositResponse = await bridge.DepositAsync(providerDepositGrpcRequest);
			if (depositResponse == null)
				return GetErrorResponse("Can't call deposit on provider bridge for request: {@request}, bridge info: {@bridgeInfo}", providerDepositGrpcRequest, bridgeInfo);

			_logger.LogDebug("Response for deposit request recieved: {@response}", depositResponse);

			if (!await UpdateDepositState(depositResponse.State, depositResponse.ExternalId, transactionId))
				return DepositGrpcResponse.Error();

			if (depositResponse.State == TransactionState.Approved)
				await _newPaymentPublisher.PublishAsync(info.ToNewPaymentServiceBusModel(transactionId));

			return DepositGrpcResponse.Ok(depositResponse);
		}

		public async ValueTask CallbackAsync(CallbackGrpcRequest request)
		{
			Guid? transactionId = request.TransactionId;

			await UpdateDepositState(request.State, request.ExternalId, transactionId);

			if (request.State == TransactionState.Approved)
			{
				PaymentDepositRepository.Grpc.Models.DepositGrpcResponse transaction = await _paymentDepositRepositoryService.Service.GetDepositAsync(new GetDepositGrpcRequest
				{
					TransactionId = transactionId
				});

				if (transaction != null)
					await _newPaymentPublisher.PublishAsync(transaction.Deposit.ToNewPaymentServiceBusModel(transaction.Deposit?.CardId));
			}
		}

		private async ValueTask<bool> UpdateDepositState(TransactionState state, string externalId, Guid? transactionId)
		{
			CommonGrpcResponse setStateResponse = await _paymentDepositRepositoryService.TryCall(service => service.SetStateAsync(new SetStateGrpcRequest
			{
				TransactionId = transactionId,
				ExternalId = externalId,
				State = state
			}));

			bool stateResponse = setStateResponse?.IsSuccess == true;
			if (stateResponse)
				_logger.LogDebug("New state for deposit: {id} setted: {state}, externalId: {externalId}", transactionId, state, externalId);
			else
				_logger.LogError("Can't update transaction state with request: {@request}", setStateResponse);

			return stateResponse;
		}

		private async ValueTask<PaymentInfo> CreatePaymentInfo(DepositGrpcRequest request)
		{
			Guid? cardId = request.CardId;
			Guid? userId = request.UserId;

			var info = new PaymentInfo
			{
				UserId = userId,
				Amount = request.Amount,
				Currency = request.Currency,
				Country = request.Country,
				ServiceCode = request.ServiceCode
			};

			if (cardId != null)
				info.CardInfo = await GetPaymentCardInfo(userId, cardId);
			else
			{
				SaveCardGrpcGrpcResponse saveResponse = await _userPaymentCardService.Service.SaveCardAsync(new SaveCardGrpcRequest
				{
					Number = request.Number,
					Holder = request.Holder,
					Month = request.Month,
					Year = request.Year,
					Cvv = request.Cvv,
					UserId = request.UserId
				});

				info.CardInfo = new PaymentCardInfo
				{
					CardId = saveResponse.CardId,
					Number = request.Number,
					Holder = request.Holder,
					Month = request.Month,
					Year = request.Year,
					Cvv = request.Cvv
				};
			}

			return info;
		}

		private async ValueTask<PaymentCardInfo> GetPaymentCardInfo(Guid? userId, Guid? cardId)
		{
			var cardInfo = new PaymentCardInfo
			{
				CardId = cardId
			};

			CardGrpcResponse card = await _userPaymentCardService.Service.GetCardAsync(new GetCardGrpcRequest { UserId = userId, CardId = cardId });

			if (card?.Number != null)
			{
				cardInfo.Number = card.Number;
				cardInfo.Holder = card.Holder;
				cardInfo.Month = card.Month;
				cardInfo.Year = card.Year;
				cardInfo.Cvv = card.Cvv;
			}

			return cardInfo;
		}

		private DepositGrpcResponse GetErrorResponse(string message, params object[] objs)
		{
			_logger.LogError(message, objs);

			return DepositGrpcResponse.Error();
		}
	}
}