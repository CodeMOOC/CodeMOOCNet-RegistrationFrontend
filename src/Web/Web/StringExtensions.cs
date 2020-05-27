using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CodeMooc.Web {

    public static class StringExtensions {

        public static string EscapeCsv(this string s) {
            if(s == null) {
                return string.Empty;
            }
            return "\"" + s.Replace("\"", "") + "\"";
        }

    }

}
