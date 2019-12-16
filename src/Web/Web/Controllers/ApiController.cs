using CodeMooc.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CodeMooc.Web.Controllers {

    [Route("api")]
    [Authorize(Policy = Startup.LegacyBasicAdministratorsPolicyName)]
    public class ApiController : Controller {

        protected DataContext Database { get; }
        protected ILogger<ApiController> Logger { get; }

        public ApiController(
            DataContext database,
            ILogger<ApiController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpPost("members/verify")]
        [HttpOptions("members/verify")]
        [Produces("application/json")]
        [EnableCors(Startup.CorsPolicyCodeMooc)]
        public IActionResult VerifyMembership([FromQuery] string email) {
            Logger.LogInformation(LoggingEvents.Api, "Member verification for mail {0}", email);

            var entry = (from e in Database.Emails
                         where e.Address == email.ToLowerInvariant().Trim()
                         select e)
                         .SingleOrDefault();
            if(entry == null) {
                return NotFound();
            }

            var user = (from u in Database.Registrations
                        where u.Id == entry.RegistrationId
                        select u)
                        .Include(u => u.Emails)
                        .First();
            var emails = (from e in user.Emails
                          orderby e.IsPrimary descending
                          select e.Address).ToList();

            var badges = (from b in Database.Badges
                          where b.Type == BadgeType.Member
                          where emails.Contains(b.Email)
                          orderby b.Year ascending
                          select b);

            var memberships = badges.ToDictionary(
                b => b.Year.Year,
                b => new {
                    IssuedOn = b.IssueTimestamp
                }
            );
            return Json(new {
                user.Id,
                PrimaryMail = emails[0],
                RegisteredOn = user.RegistrationTimestamp,
                IsMember = memberships.ContainsKey(DateTime.UtcNow.Year),
                Memberships = memberships
            });
        }

    }

}
