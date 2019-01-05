using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeMooc.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("")]
    public class RegisterController : Controller {

        protected DatabaseManager Database { get; }
        protected ILogger<RegisterController> Logger { get; }

        public RegisterController(
            DatabaseManager database,
            ILogger<RegisterController> logger
        ) {
            Database = database;
            Logger = logger;
        }

        [HttpGet]
        public IActionResult Index() {
            return View("Create");
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult Process(RegistrationViewModel model) {
            Logger.LogInformation(LoggingEvents.Registration, "Received registration request for email {0}", model.Email);

            if(!ModelState.IsValid) {
                Logger.LogInformation(LoggingEvents.Registration, "Model binding failed");
                return View("Create", model);
            }

            return Ok(true);
        }

        [HttpPost("verifica")]
        public IActionResult Validate([FromRoute] int id, [FromQuery] string secret) {
            return Ok(false);
        }

    }

}
