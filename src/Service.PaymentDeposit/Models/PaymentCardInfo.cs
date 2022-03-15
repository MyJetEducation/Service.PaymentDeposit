using System;

namespace Service.PaymentDeposit.Models
{
	public class PaymentCardInfo
	{
		public Guid? CardId { get; set; }

		public string Number { get; set; }

		public string Holder { get; set; }

		public string Month { get; set; }

		public string Year { get; set; }

		public string Cvv { get; set; }
	}
}