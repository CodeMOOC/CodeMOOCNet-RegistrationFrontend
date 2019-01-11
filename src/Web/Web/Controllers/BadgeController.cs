using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("badge")]
    public class BadgeController : Controller {

        protected DatabaseManager Database { get; }
        protected ILogger<BadgeController> Logger { get; }

        public BadgeController(
            DatabaseManager database,
            ILogger<BadgeController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpGet("{type}/criteri")]
        public IActionResult ShowCriteria([FromRoute] string type) {
            if (!type.TryParseBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            return View("Criteria", badgeType);
        }

        [HttpGet("{type}/evidence/{token}")]
        public IActionResult ShowEvidence([FromRoute] string type, [FromRoute] string token) {
            if (!type.TryParseBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            var badge = (from b in Database.Context.Badges
                         where b.Type == badgeType
                         where b.EvidenceToken == token
                         select b).SingleOrDefault();
            if(badge == null) {
                return NotFound();
            }

            var user = (from u in Database.Context.Donations
                        where u.Email == badge.Email
                        select u).FirstOrDefault();
            if(user == null) {
                Logger.LogError(LoggingEvents.Badges, "Donation for badge email {0} not found", badge.Email);
                return NotFound();
            }

            return View("Evidence", new BadgeEvidenceViewModel {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                BadgeType = badge.Type,
                IssueTimestamp = badge.IssueTimestamp
            });
        }

    }

}
