namespace Service.PaymentDeposit.Postgres.Models
{
    public class PaymentDepositEntity
    {
        public int? Id { get; set; }

        public DateTime? Date { get; set; }

        public string Value { get; set; }
    }
}
