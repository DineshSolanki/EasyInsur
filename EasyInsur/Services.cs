using EasyInsur.Modules;
using HandyControl.Tools;
using PhoneNumbers;

namespace EasyInsur
{
    internal class Services
    {
        public static AppConfig Settings = GlobalDataHelper.Load<AppConfig>();
        public static PhoneNumberUtil PhoneNumberUtil = PhoneNumberUtil.GetInstance();
    }
}