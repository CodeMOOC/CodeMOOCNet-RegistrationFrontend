using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("")]
    public class StaticController : Controller {

        protected ILogger<StaticController> Logger { get; }

        public StaticController(
            ILogger<StaticController> logger
        ) {
            Logger = logger;
        }

        [HttpGet]
        public IActionResult ShowFront() {
            return View("Front");
        }

        [HttpGet("statuto")]
        public IActionResult ShowStatute() {
            return View("Statute");
        }

    }

}
