using RepoDb.Attributes;

namespace EasyInsur.Models
{
    public class Insurance
    {
        [Identity] // Identity decoration
        [Primary]
        public long? Id { get; set; } = null;

        public string RegDate { get; set; }
        public string VehicleNo { get; set; }
        public double Amount { get; set; }

        public System.DateTime? RegDateTime
        {
            get
            {
                if (System.DateTime.TryParse(RegDate, out var dt))
                    return dt;
                return null;
            }
        }
    }
}
