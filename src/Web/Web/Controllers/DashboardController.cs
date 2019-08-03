﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("dashboard")]
    [Authorize(Policy = Startup.MembersOnlyPolicyName)]
    public class DashboardController : Controller {

        protected DatabaseManager Database { get; }
        protected ILogger<DashboardController> Logger { get; }

        public DashboardController(
            DatabaseManager database,
            ILogger<DashboardController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        protected T GetViewModel<T>()
            where T : DashboardBaseViewModel {
            T model = (T)Activator.CreateInstance(typeof(T));

            if(!HttpContext.User.Identity.IsAuthenticated) {
                Logger.LogError("User not authenticated on dashboard");
                return model;
            }

            var sId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!int.TryParse(sId, out int id)) {
                Logger.LogError("Cannot parse user ID '{0}'", sId);
                return model;
            }

            model.LoggedUser = (from r in Database.Context.Registrations
                                where r.Id == id
                                select r).SingleOrDefault();
            Logger.LogDebug("Loaded user information for user {0} {1}", model.LoggedUser.Id, model.LoggedUser.Name);

            return model;
        }

        public IActionResult Index() {
            return View(GetViewModel<DashboardBaseViewModel>());
        }

        [HttpGet("donazioni")]
        public IActionResult ShowDonations() {
            var model = GetViewModel<DashboardDonationsViewModel>();

            var emails = (from e in Database.Context.Emails
                          where e.RegistrationId == model.LoggedUser.Id
                          select e).ToArray();

            var donations = (from d in Database.Context.Donations
                             where emails.Any(e => e.Address == d.Email)
                             select d).ToList();

            model.Donations = donations;

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
