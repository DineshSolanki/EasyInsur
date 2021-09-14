using RepoDb.Attributes;

namespace EasyInsur.Models
{
    public class Transactions
    {
        public Transactions()
        {
        }

        public Transactions(long? insuranceId, long? personId, double fixedAmount, double od, double tp, double tax, double totalAmount, string commissionType, double commissionAmount, double odPercent, double tpPercent, double afterCommissionAmount, double payment, string paymentDate, double balance, double previousBalance, double finalBalance, long? id = null)
        {
            Id = id;
            InsuranceID = insuranceId;
            PersonID = personId;
            FixedAmount = fixedAmount;
            OD = od;
            TP = tp;
            Tax = tax;
            TotalAmount = totalAmount;
            CommissionType = commissionType;
            CommissionAmount = commissionAmount;
            ODPercent = odPercent;
            TPPercent = tpPercent;
            AfterCommissionAmount = afterCommissionAmount;
            Payment = payment;
            PaymentDate = paymentDate;
            Balance = balance;
            PreviousBalance = previousBalance;
            FinalBalance = finalBalance;
        }

        [Identity] // Identity decoration
        public long? Id { get; set; } = null;

        public long? InsuranceID { get; set; }
        public long? PersonID { get; set; }
        public double FixedAmount { get; set; }
        public double OD { get; set; }
        public double TP { get; set; }
        public double Tax { get; set; }
        public double TotalAmount { get; set; }
        public string CommissionType { get; set; }
        public double CommissionAmount { get; set; }
        public double ODPercent { get; set; }
        public double TPPercent { get; set; }
        public double AfterCommissionAmount { get; set; }
        public double Payment { get; set; }
        public string PaymentDate { get; set; }
        public double Balance { get; set; }
        public double PreviousBalance { get; set; }
        public double FinalBalance { get; set; }
    }
}
