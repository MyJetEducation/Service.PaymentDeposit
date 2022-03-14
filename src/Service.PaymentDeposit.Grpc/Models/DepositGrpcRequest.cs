using System;
using System.Runtime.Serialization;

namespace Service.PaymentDeposit.Grpc.Models
{
    [DataContract]
    public class DepositGrpcRequest
    {
		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }

		[DataMember(Order = 2)]
		public decimal Amount { get; set; }

		[DataMember(Order = 3)]
		public string Currency { get; set; }

		[DataMember(Order = 4)]
		public string Country { get; set; }

		[DataMember(Order = 5)]
		public string ServiceCode { get; set; }

		[DataMember(Order = 6)]
		public string Number { get; set; }

		[DataMember(Order = 7)]
		public string Holder { get; set; }

		[DataMember(Order = 8)]
		public string Month { get; set; }

		[DataMember(Order = 9)]
		public string Year { get; set; }

		[DataMember(Order = 10)]
		public string Cvv { get; set; }
	}
}
