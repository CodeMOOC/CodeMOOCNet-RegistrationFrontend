using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMooc.Web.Controllers {

    [Route("dashboard")]
    [Authorize(Policy = Startup.MembersOnlyPolicyName)]
    public class DashboardController : Controller {

        public IActionResult Index() {
            return Content("You are logged, my dude");
        }

    }

}
