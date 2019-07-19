using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace CodeMooc.Web {

    public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions {

        public const string DefaultScheme = "Basic auth";

        public string Scheme => DefaultScheme;

        public StringValues AuthKey { get; set; }

    }

}
