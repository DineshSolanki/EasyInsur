using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using HandyControlWpfCoreApp1.Models;
using Newtonsoft.Json;
using System.Reflection;

namespace HandyControlWpfCoreApp1.Modules
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
        

    }
}
