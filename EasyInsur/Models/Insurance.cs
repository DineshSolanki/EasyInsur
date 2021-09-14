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
    }
}
