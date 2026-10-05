namespace EasyInsur.Models
{
    public class DashboardActivityItem
    {
        public long Id { get; set; }
        public string PaymentDate { get; set; } = string.Empty;
        public string PayeeName { get; set; } = string.Empty;
        public string PayeeType { get; set; } = string.Empty;
        public string VehicleNo { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public double CommissionAmount { get; set; }
        public double Payment { get; set; }
        public double Balance { get; set; }
    }
}
