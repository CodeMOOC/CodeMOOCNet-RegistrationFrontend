using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeMooc.Web.Data;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("dashboard")]
    [Authorize(Policy = Startup.MembersOnlyPolicyName)]
    public class DashboardController : Controller {

        protected readonly DataContext Database;
        protected readonly ILogger<DashboardController> Logger;
        protected readonly IConfiguration Configuration;

        public DashboardController(
            DataContext database,
            ILogger<DashboardController> logger,
            IConfiguration configuration
        ) {
            Database = database;
            Logger = logger;
            Configuration = configuration;
        }

        protected T GetViewModel<T>()
            where T : DashboardBaseViewModel {
            T model = (T)Activator.CreateInstance(typeof(T));

            if(!HttpContext.User.Identity.IsAuthenticated) {
                Logger.LogError("User not authenticated on dashboard");
                return null;
            }

            var sId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!int.TryParse(sId, out int id)) {
                Logger.LogError("Cannot parse user ID '{0}'", sId);
                return null;
            }

            model.LoggedUser = (from r in Database.Registrations
                                where r.Id == id
                                select r).SingleOrDefault();
            Logger.LogDebug("Loaded user information for user {0} {1}", model.LoggedUser.Id, model.LoggedUser.Name);

            return model;
        }

        public IActionResult Index() {
            var model = GetViewModel<DashboardBaseViewModel>();
            if(model == null) {
                return Forbid();
            }

            return View(model);
        }

        [HttpGet("donazioni")]
        public IActionResult ShowDonations() {
            var model = GetViewModel<DashboardDonationsViewModel>();
            if(model == null) {
                return Forbid();
            }

            var emails = (from e in Database.Emails
                          where e.RegistrationId == model.LoggedUser.Id
                          select e)
                          .Include(e => e.AssociatedDonations)
                          .Single();

            model.Donations = emails.AssociatedDonations;

            return View("Donations", model);
        }

        [HttpGet("stato")]
        public IActionResult ShowAssociationStatus() {
            return null;
        }

        [HttpGet("cv-upload")]
        public IActionResult ShowCvUpload() {
            return null;
        }

        [HttpPost("cv-upload")]
        public IActionResult ProcessCvUpload() {
            return null;
        }

    }

}
