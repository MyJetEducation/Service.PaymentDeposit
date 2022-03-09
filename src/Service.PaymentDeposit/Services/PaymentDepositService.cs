using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDeposit.Grpc;
using Service.PaymentDeposit.Grpc.Models;
using Service.PaymentDeposit.Mappers;
using Service.PaymentDeposit.Models;
using Service.PaymentDepositRepository.Grpc;
using Service.PaymentDepositRepository.Grpc.Models;

namespace Service.PaymentDeposit.Services
{
	public class PaymentDepositService : IPaymentDepositService
	{
		private readonly ILogger<PaymentDepositService> _logger;
		private readonly IPaymentProviderRouter _paymentProviderRouter;
		private readonly IPaymentProviderResolver _paymentProviderResolver;
		private readonly IGrpcServiceProxy<IPaymentDepositRepositoryService> _paymentDepositRepositoryService;

		public PaymentDepositService(ILogger<PaymentDepositService> logger,
			IPaymentProviderRouter paymentProviderRouter,
			IPaymentProviderResolver paymentProviderResolver,
			IGrpcServiceProxy<IPaymentDepositRepositoryService> paymentDepositRepositoryService)
		{
			_logger = logger;
			_paymentProviderRouter = paymentProviderRouter;
			_paymentProviderResolver = paymentProviderResolver;
			_paymentDepositRepositoryService = paymentDepositRepositoryService;
		}

		public async ValueTask<DepositGrpcResponse> DepositAsync(DepositGrpcRequest request)
		{
			PaymentProviderBridgeInfo bridgeInfo = _paymentProviderRouter.GetPaymentProviderBridge(request);
			if (bridgeInfo == null)
			{
				_logger.LogError("Can't find deposit provider for request: {request}", request);

				return new DepositGrpcResponse();
			}

			_logger.LogDebug("PaymentProviderBridgeInfo recieved: {info}", bridgeInfo);

			IPaymentProviderGrpcService providerBridge = _paymentProviderResolver.GetProviderBridge(bridgeInfo);
			if (providerBridge == null)
			{
				_logger.LogError("Can't create provider bridge for request: {request}, bridge info: {bridgeInfo}", request, bridgeInfo);

				return new DepositGrpcResponse();
			}

			_logger.LogDebug("PaymentProviderGrpcService connection created: {providerBridge}", providerBridge);

			RegisterGrpcResponse registerGrpcResponse = await _paymentDepositRepositoryService.TryCall(service => service.RegisterAsync(request.ToGrpcModel(bridgeInfo)));
			if (registerGrpcResponse == null)
			{
				_logger.LogError("Can't register deposit transaction for request: {request}, bridge info: {bridgeInfo}", request, bridgeInfo);

				return new DepositGrpcResponse();
			}

			Guid? transactionId = registerGrpcResponse.TransactionId;
			_logger.LogDebug("Transaction registered with id: {id}", transactionId);

			ProviderDepositGrpcRequest providerDepositGrpcRequest = request.ToGrpcModel(transactionId);
			ProviderDepositGrpcResponse depositResponse = await providerBridge.DepositAsync(providerDepositGrpcRequest);
			if (depositResponse == null)
			{
				_logger.LogError("Can't call deposit on provider bridge for request: {request}, bridge info: {bridgeInfo}", providerDepositGrpcRequest, bridgeInfo);

				return new DepositGrpcResponse();
			}

			_logger.LogDebug("Response for deposit request recieved: {response}", depositResponse);

			string externalId = depositResponse.ExternalId;
			if (externalId != null)
			{
				CommonGrpcResponse setStateResponse = await _paymentDepositRepositoryService.TryCall(service => service.SetStateAsync(new SetStateGrpcRequest
				{
					TransactionId = transactionId,
					ExternalId = externalId,
					State = depositResponse.State
				}));

				bool setStateResult = setStateResponse?.IsSuccess == false;
				if (!setStateResult)
					_logger.LogError("Can't update transaction state with request: {request}", setStateResponse);

				_logger.LogDebug("New state for deposit: {id} setted: {state}, externalId: {externalId}", transactionId, depositResponse.State, externalId);

				return new DepositGrpcResponse
				{
					Approved = setStateResult
				};
			}

			return new DepositGrpcResponse
			{
				RedirectUrl = depositResponse.RedirectUrl
			};
		}

		public async ValueTask CallbackAsync(CallbackGrpcRequest request)
		{
			CommonGrpcResponse response = await _paymentDepositRepositoryService.TryCall(service => service.SetStateAsync(new SetStateGrpcRequest
			{
				TransactionId = request.TransactionId,
				ExternalId = request.ExternalId,
				State = request.State
			}));

			if (response?.IsSuccess == false)
				_logger.LogError("Can't update transaction state with request: {request}", response);
			else
				_logger.LogDebug("New state (from callback) for deposit: {id} setted: {state}, externalId: {externalId}", request.TransactionId, request.State, request.ExternalId);
		}
	}
}