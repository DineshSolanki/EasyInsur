using System.Text.Json;
using HandyControl.Tools;

namespace EasyInsur.Modules
{
    internal class AppConfig : GlobalDataHelper
    {
        public string ConnectionString = $"Data Source={Services.AppPathWithoutName}/EasyInsur.db;Version=3;";
        public string OwnerName { get; set; } = "Agency Owner Name";
        public string OwnerPhone { get; set; } = "0000000000";
        public string OwnerEmail { get; set; } = "owner@example.com";
        public override string FileName { get; set; }
        public override JsonSerializerOptions JsonSerializerOptions { get; set; }
        public override int FileVersion { get; set; }
    }
}