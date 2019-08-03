using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Model {

    public class LoginViewModel {

        public enum LoginStatus {
            None,
            LoggedOut,
            LoginFailure
        }

        public bool IsLoggedIn { get; set; }

        public string Email { get; set; }

        public LoginStatus Status { get; set; }

        public string ProceedUrl { get; set; }

    }

}
