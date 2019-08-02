using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CodeMooc.Web {

    public class StaticUserIdentity : ClaimsIdentity {

        public StaticUserIdentity(string username)
            : base(new Claim[] {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, Startup.AdministratorRole),
                new Claim(ClaimTypes.Role, Startup.MemberRole)
            }, BasicAuthenticationSchemeOptions.SchemeName) {
            
        }

    }

}
