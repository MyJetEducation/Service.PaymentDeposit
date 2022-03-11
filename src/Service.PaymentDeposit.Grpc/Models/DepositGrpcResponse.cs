using System.Runtime.Serialization;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDepositRepository.Domain.Models;

namespace Service.PaymentDeposit.Grpc.Models
{
	[DataContract]
	public class DepositGrpcResponse
	{
		[DataMember(Order = 1)]
		public TransactionState State { get; set; }

		[DataMember(Order = 2)]
		public string RedirectUrl { get; set; }

		public static DepositGrpcResponse Error() => new() {State = TransactionState.Error};

		public static DepositGrpcResponse Ok(TransactionState state, string redirectUrl = null) => new() {State = state, RedirectUrl = redirectUrl};

		public static DepositGrpcResponse Ok(ProviderDepositGrpcResponse depositResponse) => new() {State = depositResponse.State, RedirectUrl = depositResponse.RedirectUrl};
	}
}