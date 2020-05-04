using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Data {
    public static class DataExtensions {

        public static bool IsPersonalAddressComplete(this PignaNotebookRegistration r) {
            return (
                !string.IsNullOrEmpty(r.PersonalAddress) &&
                !string.IsNullOrEmpty(r.PersonalCap) &&
                !string.IsNullOrEmpty(r.PersonalCity) &&
                !string.IsNullOrEmpty(r.PersonalProvince)
            );
        }

    }
}
