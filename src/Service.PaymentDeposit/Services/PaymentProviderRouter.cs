using System.Collections.Concurrent;
using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDeposit.Models;

namespace Service.PaymentDeposit.Services
{
	public class PaymentProviderRouter : IPaymentProviderRouter
	{
		private static readonly ConcurrentDictionary<PaymentProviderBridgeInfo, IPaymentProviderGrpcService> GrpcServices;

		static PaymentProviderRouter() => GrpcServices = new ConcurrentDictionary<PaymentProviderBridgeInfo, IPaymentProviderGrpcService>();

		public IPaymentProviderGrpcService GetProviderBridge(PaymentProviderBridgeInfo bridgeInfo) =>
			GrpcServices.GetOrAdd(bridgeInfo, info => GrpcChannel.ForAddress(info.Url).CreateGrpcService<IPaymentProviderGrpcService>());
	}
}