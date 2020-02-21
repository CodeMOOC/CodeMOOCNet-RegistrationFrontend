using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace CodeMooc.Web {

    public static class TempDataExtensions {

        public static void Put<T>(this ITempDataDictionary tempData, T value)
            where T : class {
            var key = typeof(T).Name;
            tempData[key] = JsonSerializer.Serialize<T>(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData) where T : class {
            var key = typeof(T).Name;
            tempData.TryGetValue(key, out object o);

            string value = o as string;
            return value == null ? null : JsonSerializer.Deserialize<T>(value);
        }

    }

}
