using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
				return GetErrorResponse("Can't find deposit provider for request: {request}", request);

			_logger.LogDebug("PaymentProviderBridgeInfo recieved: {info}", JsonConvert.SerializeObject(bridgeInfo));

			IPaymentProviderGrpcService providerBridge = _paymentProviderResolver.GetProviderBridge(bridgeInfo);
			if (providerBridge == null)
				return GetErrorResponse("Can't create provider bridge for request: {request}, bridge info: {bridgeInfo}", request, bridgeInfo);

			_logger.LogDebug("PaymentProviderGrpcService connection created: {providerBridge}", JsonConvert.SerializeObject(providerBridge));

			RegisterGrpcResponse registerGrpcResponse = await _paymentDepositRepositoryService.TryCall(service => service.RegisterAsync(request.ToGrpcModel(bridgeInfo)));
			if (registerGrpcResponse == null)
				return GetErrorResponse("Can't register deposit transaction for request: {request}, bridge info: {bridgeInfo}", request, bridgeInfo);

			Guid? transactionId = registerGrpcResponse.TransactionId;
			_logger.LogDebug("Transaction registered with id: {id}", transactionId);

			ProviderDepositGrpcRequest providerDepositGrpcRequest = request.ToGrpcModel(transactionId);
			ProviderDepositGrpcResponse depositResponse = await providerBridge.DepositAsync(providerDepositGrpcRequest);
			if (depositResponse == null)
				return GetErrorResponse("Can't call deposit on provider bridge for request: {request}, bridge info: {bridgeInfo}", providerDepositGrpcRequest, bridgeInfo);

			_logger.LogDebug("Response for deposit request recieved: {response}", JsonConvert.SerializeObject(depositResponse));

			if (!await UpdateDepositState(depositResponse.State, depositResponse.ExternalId, transactionId))
				return DepositGrpcResponse.Error();

			return DepositGrpcResponse.Ok(depositResponse);
		}

		public async ValueTask CallbackAsync(CallbackGrpcRequest request) => await UpdateDepositState(request.State, request.ExternalId, request.TransactionId);

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
				_logger.LogError("Can't update transaction state with request: {request}", setStateResponse);

			return stateResponse;
		}

		private DepositGrpcResponse GetErrorResponse(string message, params object[] objs)
		{
			_logger.LogError(message, objs);

			return DepositGrpcResponse.Error();
		}
	}
}