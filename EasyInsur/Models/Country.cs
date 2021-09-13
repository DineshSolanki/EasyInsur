namespace HandyControlWpfCoreApp1.Models
{
    public class Country
    {
        public string name { get; set; }
        public string dial_code { get; set; }
        public string code { get; set; }
        public override string ToString() => name;
    }
}
