using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EasyInsur.Models;
using HandyControl.Tools;
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
                return new Tuple<string, string>(mask, formattedNumber);
            }
            catch (Exception)
            {
                return new Tuple<string, string>("", "");
            }
        }
        /// <summary>
        /// Starts Process associated with given path.
        /// </summary>
        /// <param name="path">if path is a URL it opens url in default browser, 
        /// if path is File Or folder path it will be started.</param>
        public static void StartProcess(string path)
        {
            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true
            });
        }
        public static IEnumerable<T> Join<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first == null ? second : second == null ? first : first.Concat(second).ToList();
        }

        public static string Rtolistpdf = Path.Join(Path.GetDirectoryName(ApplicationHelper.GetExecutablePathNative()),"Resources","rtolist.pdf");
    }
}
