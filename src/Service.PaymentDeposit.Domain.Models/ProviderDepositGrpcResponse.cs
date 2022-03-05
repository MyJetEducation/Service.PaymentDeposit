using System.Runtime.Serialization;
using Service.PaymentDepositRepository.Domain.Models;

namespace Service.PaymentDeposit.Domain.Models
{
	[DataContract]
	public class ProviderDepositGrpcResponse
	{
		public TransactionState State { get; set; }

		public string ExternalId { get; set; }

		public string RedirectUrl { get; set; }
	}
}