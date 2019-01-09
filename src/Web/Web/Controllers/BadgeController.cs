using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if(!type.TryParseBadgeType(out BadgeType badgeType)) {
                return NotFound();
            }

            return View("Criteria", badgeType);
        }

    }

}
