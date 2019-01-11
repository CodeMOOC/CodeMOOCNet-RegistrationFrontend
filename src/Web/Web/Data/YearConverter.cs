using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CodeMooc.Web.Data {

    public static class YearConverter {

        public static ValueConverter Create() {
            return new ValueConverter<DateTime, string>(
                input => input.Year.ToString(),
                input => new DateTime(Convert.ToInt32(input), 1, 1)
            );
        }

    }

}
