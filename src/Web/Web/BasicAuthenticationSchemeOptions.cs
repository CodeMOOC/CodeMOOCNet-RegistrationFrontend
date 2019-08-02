using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace CodeMooc.Web {

    public class BasicAuthenticationSchemeOptions : AuthenticationSchemeOptions {

        public const string SchemeName = "Custom basic auth";

        public string Scheme => SchemeName;

        public StringValues AuthKey { get; set; }

    }

}
