using System;

namespace Service.PaymentDeposit.Models
{
	public class PaymentInfo
	{
		public Guid? UserId { get; set; }

		public decimal Amount { get; set; }

		public string Currency { get; set; }

		public string Country { get; set; }

		public string ServiceCode { get; set; }

		public PaymentCardInfo CardInfo { get; set; }
	}
}