using System;
using System.Linq;
using CodeMooc.Web.Data;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("badge")]
    [AllowAnonymous]
    public class BadgeController : Controller {

        protected DataContext Database { get; }
        protected ILogger<BadgeController> Logger { get; }

        public BadgeController(
            DataContext database,
            ILogger<BadgeController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        public const int MinYear = 2019;
        public const int MaxYear = 2020;

        [HttpGet("{type}/criteri")]
        public IActionResult ShowCriteria([FromRoute] string type) {
            Logger.LogDebug(LoggingEvents.Badges, "Loading criteria for badge type {0}", type);

            if(!type.TryParseLegacyBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            return View("Criteria", new BadgeCriteriaViewModel {
                BadgeType = badgeType,
                Year = 2019
            });
        }

        [HttpGet("{type}/{year}/criteri")]
        public IActionResult ShowCriteriaYear([FromRoute] string type, [FromRoute] int year) {
            Logger.LogDebug(LoggingEvents.Badges, "Loading criteria for badge type {0} year {1}", type, year);

            if(!type.TryParseBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            if(year < MinYear || year > MaxYear) {
                return NotFound();
            }

            return View("Criteria", new BadgeCriteriaViewModel {
                BadgeType = badgeType,
                Year = year
            });
        }

        [HttpGet("{type}/evidence/{token}")]
        public IActionResult ShowEvidence([FromRoute] string type, [FromRoute] string token) {
            Logger.LogDebug(LoggingEvents.Badges, "Loading evidence for badge type {0}", type);

            if (!type.TryParseLegacyBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            return ShowEvidenceInternal(badgeType, 2019, token);
        }

        [HttpGet("{type}/{year}/evidence/{token}")]
        public IActionResult ShowEvidenceYear([FromRoute] string type, [FromRoute] int year, [FromRoute] string token) {
            Logger.LogDebug(LoggingEvents.Badges, "Loading evidence for badge type {0} year {1}", type, year);

            if(!type.TryParseBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            if(year < MinYear || year > MaxYear) {
                return NotFound();
            }

            return ShowEvidenceInternal(badgeType, year, token);
        }

        protected IActionResult ShowEvidenceInternal(BadgeType badgeType, int year, string token) {
            var badge = (from b in Database.Badges
                         where b.Type == badgeType
                         where b.Year == new DateTime(year, 1, 1)
                         where b.EvidenceToken == token
                         select b).SingleOrDefault();
            if(badge == null) {
                Logger.LogDebug(LoggingEvents.Badges, "Badge of type {0} year {1} and token '{2}' not found", badgeType, year, token);
                return NotFound();
            }

            var user = (from u in Database.Donations
                        where u.Email == badge.Email
                        where u.Year == new DateTime(year, 1, 1)
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
                Year = year,
                IssueTimestamp = badge.IssueTimestamp
            });
        }

    }

}
