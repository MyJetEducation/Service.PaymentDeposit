using System.Runtime.Serialization;
using Service.PaymentDepositRepository.Domain.Models;

namespace Service.PaymentDeposit.Domain.Models
{
	[DataContract]
	public class ProviderDepositGrpcResponse
	{
		[DataMember(Order = 1)]
		public TransactionState State { get; set; }

		[DataMember(Order = 2)]
		public string ExternalId { get; set; }

		[DataMember(Order = 3)]
		public string RedirectUrl { get; set; }
	}
}