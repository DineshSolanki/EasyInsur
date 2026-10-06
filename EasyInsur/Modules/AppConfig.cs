using System.Text.Json;
using HandyControl.Tools;

namespace EasyInsur.Modules
{
    public class AppConfig : GlobalDataHelper
    {
        public string ConnectionString { get; set; } = $"Data Source={Services.AppPathWithoutName}/EasyInsur.db;Version=3;Foreign Keys=True;";
        public string OwnerName { get; set; } = "Agency Owner Name";
        public string OwnerPhone { get; set; } = "0000000000";
        public string OwnerEmail { get; set; } = "owner@example.com";
        public string SyncfusionLicenseKey { get; set; } = "";
        public override string FileName { get; set; } = "AppConfig.json";
        public override JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions { WriteIndented = true };
        public override int FileVersion { get; set; } = 1;
    }
}