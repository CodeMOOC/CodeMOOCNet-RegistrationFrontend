using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CodeMooc.Web {

    public class StaticUserIdentity : GenericIdentity {

        private readonly string Username;

        public StaticUserIdentity(string username) : base(username, "User") {
            Username = username;
        }

    }

}
