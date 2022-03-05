using System.Runtime.Serialization;

namespace Service.PaymentDeposit.Grpc.Models
{
    [DataContract]
    public class DepositGrpcResponse
    {
        [DataMember(Order = 1)]
        public bool Approved { get; set; }

		[DataMember(Order = 2)]
        public string RedirectUrl { get; set; }
    }
}
