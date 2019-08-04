using CodeMooc.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var users = from r in Database.Registrations
                        orderby r.Id ascending
                        select r;

            var sb = new StringBuilder();
            sb.AppendLine("# ID, Name, Surname, FiscalCode, Category, RegisteredOn, Confirmed");
            foreach (var u in users) {
                sb.AppendJoin(',',
                    u.Id,
                    u.Name,
                    u.Surname,
                    u.FiscalCode.ToUpperInvariant(),
                    u.Category,
                    u.RegistrationTimestamp.ToString("R"),
                    u.ConfirmationTimestamp.HasValue ? "1" : "0"
                );
                sb.AppendLine();
            }

            Response.Headers.TryAdd(HeaderNames.ContentDisposition, new StringValues("attachment; filename=registrations.csv"));

            return Content(sb.ToString(), "text/csv");
        }

    }

}
