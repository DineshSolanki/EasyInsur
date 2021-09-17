using System.IO;
using EasyInsur.Modules;
using HandyControl.Tools;
using PhoneNumbers;

namespace EasyInsur
{
    internal class Services
    {
        public static string AppPathWithoutName = Path.GetDirectoryName(ApplicationHelper.GetExecutablePathNative())!;
        public static AppConfig Settings = GlobalDataHelper.Load<AppConfig>();
        public static PhoneNumberUtil PhoneNumberUtil = PhoneNumberUtil.GetInstance();
        
    }
}