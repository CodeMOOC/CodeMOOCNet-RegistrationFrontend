using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace CodeMooc.Web {

    public static class TempDataExtensions {

        public static void Put<T>(this ITempDataDictionary tempData, T value)
            where T : class {
            var key = typeof(T).Name;
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData) where T : class {
            var key = typeof(T).Name;
            tempData.TryGetValue(key, out object o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

    }

}
