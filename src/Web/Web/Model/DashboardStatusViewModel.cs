using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeMooc.Web.Data;

namespace CodeMooc.Web.Model {

    public class DashboardStatusViewModel : DashboardBaseViewModel {

        public IList<Email> Emails { get; set; }
        public Email PrimaryEmail { get; set; }

        public bool IsAssociateForCurrentYear { get; set; }

        public bool IsRegistrationConfirmed { get; set; }

        public IList<Badge> Badges { get; set; }

        public string ConfirmationEmailAddress { get; set; }

    }

}
