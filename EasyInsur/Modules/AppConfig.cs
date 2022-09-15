using System.Text.Json;
using HandyControl.Tools;

namespace EasyInsur.Modules
{
    internal class AppConfig : GlobalDataHelper
    {
        // public string ConnectionString = $@"Data Source={Services.AppPathWithoutName}/EasyInsur.db;Version=3;";
        public string ConnectionString = @"Server=remotemysql.com;Database=WEykKpOXU9;Uid=WEykKpOXU9;password=05nlcXkQZA;";
        public override string FileName { get; set; }
        public override JsonSerializerOptions JsonSerializerOptions { get; set; }
        public override int FileVersion { get; set; }
    }
}