using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("")]
    public class StaticController : Controller {

        protected DatabaseManager Database { get; }
        protected ILogger<StaticController> Logger { get; }

        public StaticController(
            DatabaseManager database,
            ILogger<StaticController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpGet("statuto")]
        public IActionResult ShowStatute() {
            return View("Statute");
        }

    }

}
