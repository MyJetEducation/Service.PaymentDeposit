using System;
using System.Runtime.Serialization;
using Service.PaymentDepositRepository.Domain.Models;

namespace Service.PaymentDeposit.Grpc.Models
{
	[DataContract]
	public class CallbackGrpcRequest
	{
		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }
		
		[DataMember(Order = 2)]
		public Guid? TransactionId { get; set; }
		
		[DataMember(Order = 3)]
		public string ExternalId { get; set; }	
		
		[DataMember(Order = 4)]
		public TransactionState State { get; set; }
	}
}