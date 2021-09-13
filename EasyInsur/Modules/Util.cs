using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using HandyControlWpfCoreApp1;
using HandyControlWpfCoreApp1.Models;
using HandyControlWpfCoreApp1.Modules;
using Newtonsoft.Json;
using PhoneNumbers;

namespace EasyInsur.Modules
{
    public static class Util
    {
        public static double GetPercentageOf(double percentToCalculate, double number)
        {
            return number * percentToCalculate / 100;
        }
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> col)
        {
            return new ObservableCollection<T>(col);
        }
        public static ObservableCollection<Country>? Read()
        {
            var resName = Resources.CountryDetailsJson;
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resName);

            using StreamReader reader = new(stream!);
            using JsonTextReader jsonReader = new(reader);
            JsonSerializer ser = new();

            return ser.Deserialize<ObservableCollection<Country>>(jsonReader);
        }

        public static Tuple<string, string> GetMobileDetails(string regionCode)
        {
            try
            {
                var exampleNumber = Services.PhoneNumberUtil.GetExampleNumberForType(regionCode, PhoneNumberType.MOBILE);
                var formattedNumber = Services.PhoneNumberUtil.FormatNumberForMobileDialing(exampleNumber, regionCode, true)[1..];
                var mask = Regex.Replace(formattedNumber, @"\d", "0");
                return new Tuple<string, string>(mask,formattedNumber);
            }
            catch (Exception)
            {
                return new Tuple<string, string>("", "");
            }
        }


    }
}
