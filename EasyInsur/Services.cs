using HandyControl.Tools;
using HandyControlWpfCoreApp1.Modules;
using PhoneNumbers;

namespace HandyControlWpfCoreApp1
{
    internal class Services
    {
        public static AppConfig Settings = GlobalDataHelper.Load<AppConfig>();
        public static PhoneNumberUtil PhoneNumberUtil = PhoneNumberUtil.GetInstance();
    }
}