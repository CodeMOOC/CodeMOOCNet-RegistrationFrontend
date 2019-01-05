using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web.Controllers {

    [Route("iscrizione")]
    public class RegisterController : ControllerBase {

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
        public IActionResult Get() {
            return Ok();
        }

        [HttpPost]
        public IActionResult Register() {
            return Ok(false);
        }

    }

}
