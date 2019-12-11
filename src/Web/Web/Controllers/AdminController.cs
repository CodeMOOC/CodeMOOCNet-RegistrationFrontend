using CodeMooc.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMooc.Web.Controllers {

    [Route("admin")]
    [Authorize(Policy = Startup.LegacyBasicAdministratorsPolicyName)]
    public class AdminController : Controller {

        protected DataContext Database { get; }
        protected ILogger<RegisterController> Logger { get; }

        public AdminController(
            DataContext database,
            ILogger<RegisterController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpGet("registrations")]
        public IActionResult ListRegistrations() {
            var users = (from r in Database.Registrations
                         orderby r.Id ascending
                         select r)
                         .Include(r => r.Emails);

            var sb = new StringBuilder();
            sb.AppendLine("# ID, Name, Surname, FiscalCode, Category, RegisteredOn, Confirmed, PrimaryMail");
            foreach (var u in users) {
                sb.AppendJoin(',',
                    u.Id,
                    u.Name,
                    u.Surname,
                    u.FiscalCode.ToUpperInvariant(),
                    u.Category,
                    u.RegistrationTimestamp.ToString("R"),
                    u.ConfirmationTimestamp.HasValue ? "1" : "0",
                    u.Emails.OrderByDescending(e => e.IsPrimary).FirstOrDefault()?.Address
                );
                sb.AppendLine();
            }

            Response.Headers.TryAdd(HeaderNames.ContentDisposition, new StringValues("attachment; filename=registrations.csv"));

            return Content(sb.ToString(), "text/csv");
        }

        [HttpGet("members/{year}")]
        public IActionResult ListMembers(int year) {
            var users = (from r in Database.Registrations
                         orderby r.Id ascending
                         select r)
                         .Include(r => r.Emails);

            var donations = (from b in Database.Badges
                             where b.Type == BadgeType.Member
                             where b.Year.Year == year
                             select b)
                             .ToDictionary(b => b.Email);

            var sb = new StringBuilder();
            sb.AppendLine("# ID, Name, Surname, FiscalCode, Category, RegisteredOn, Confirmed, PrimaryMail");
            foreach(var u in users) {
                if(!u.Emails.Any(e => donations.ContainsKey(e.Address))) {
                    // No e-mail matches membership badge
                    continue;
                }

                sb.AppendJoin(',',
                    u.Id,
                    u.Name,
                    u.Surname,
                    u.FiscalCode.ToUpperInvariant(),
                    u.Category,
                    u.RegistrationTimestamp.ToString("R"),
                    u.ConfirmationTimestamp.HasValue ? "1" : "0",
                    u.Emails.OrderByDescending(e => e.IsPrimary).FirstOrDefault()?.Address
                );
                sb.AppendLine();
            }

            Response.Headers.TryAdd(HeaderNames.ContentDisposition, new StringValues("attachment; filename=members.csv"));

            return Content(sb.ToString(), "text/csv");
        }

        private const int ExpirationHour = 6;
        private const int ExpirationMinutes = 30;

        [HttpGet("check-mail")]
        [Produces("application/json")]
        public IActionResult CheckEmailStatus(string email) {
            var userMail = (from e in Database.Emails
                            where e.Address == email.ToLowerInvariant()
                            select e).SingleOrDefault();
            if(userMail == null) {
                return NotFound();
            }

            var emails = (from e in Database.Emails
                          where e.RegistrationId == userMail.RegistrationId
                          select e)
                          .Include(e => e.AssociatedBadges)
                          .ToList();

            var badges = emails.SelectMany(e => e.AssociatedBadges).ToList();
            bool isAssociate = badges.Any(b => b.Year.Year == DateTime.Now.Year && b.Type == BadgeType.Member);

            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);
            var lastGeneration = (now.Hour < ExpirationHour && now.Minute < ExpirationMinutes) ?
                new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, ExpirationHour, ExpirationMinutes, 0, DateTimeKind.Utc) :
                new DateTime(now.Year, now.Month, now.Day, ExpirationHour, ExpirationMinutes, 0, DateTimeKind.Utc);

            Response.Headers.Add(HeaderNames.LastModified, lastGeneration.ToString("R"));
            Response.Headers.Add(HeaderNames.Expires, lastGeneration.AddDays(1).AddSeconds(1).ToString("R"));

            return Ok(new {
                UserId = userMail.RegistrationId,
                KnownIdentities = from e in emails
                                  select new { Email = e.Address, e.IsPrimary },
                IsMember = isAssociate,
                GeneratedBadges = from b in badges
                                  select new { Type = b.Type.GetPathToken(), b.Year.Year }
            });
        }

    }

}
