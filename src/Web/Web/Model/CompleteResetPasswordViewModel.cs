using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Model {

    public class CompleteResetPasswordViewModel {

        public int UserId { get; set; }

        public string Secret { get; set; }

        public bool PasswordReset { get; set; }

    }

}
