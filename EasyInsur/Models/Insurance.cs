using RepoDb.Attributes;

namespace HandyControlWpfCoreApp1.Models
{
    public class Insurance
    {
        [Identity] // Identity decoration
        public long? Id { get; set; } = null;

        public string RegDate { get; set; }
        public string VehicleNo { get; set; }
        public double Amount { get; set; }
    }
}
