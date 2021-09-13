using HandyControl.Tools;
using HandyControlWpfCoreApp1.Modules;

namespace HandyControlWpfCoreApp1
{
    internal class Services
    {
        public static AppConfig Settings = GlobalDataHelper.Load<AppConfig>();
    }
}