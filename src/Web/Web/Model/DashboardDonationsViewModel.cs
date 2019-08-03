using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeMooc.Web.Data;

namespace CodeMooc.Web.Model {

    public class DashboardDonationsViewModel : DashboardBaseViewModel {

        public IList<Donation> Donations { get; set; }

    }

}
