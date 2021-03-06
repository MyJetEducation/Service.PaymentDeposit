using System.Collections.Concurrent;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentProviderRouter.Grpc.Models;

namespace Service.PaymentDeposit.Services
{
	public class PaymentProviderResolver : IPaymentProviderResolver
	{
		private static readonly ConcurrentDictionary<PaymentProviderBridgeInfo, IPaymentProviderGrpcService> GrpcServices;

		static PaymentProviderResolver() => GrpcServices = new ConcurrentDictionary<PaymentProviderBridgeInfo, IPaymentProviderGrpcService>();

		public IPaymentProviderGrpcService GetProviderBridge(PaymentProviderBridgeInfo bridgeInfo) =>
			GrpcServices.GetOrAdd(bridgeInfo, info => GrpcChannel.ForAddress(info.ServiceUrl).CreateGrpcService<IPaymentProviderGrpcService>());
	}
}