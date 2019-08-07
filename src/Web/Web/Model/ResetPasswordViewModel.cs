using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Model {

    public class ResetPasswordViewModel {

        public bool PasswordResetRequested { get; set; }

        public bool FailureDuringReset { get; set; }

    }

}
