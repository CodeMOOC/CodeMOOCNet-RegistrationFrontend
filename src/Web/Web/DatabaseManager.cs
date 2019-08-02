using System;
using CodeMooc.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CodeMooc.Web {

    public class DatabaseManager {

        protected readonly IConfiguration _configuration;
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly ILogger<DatabaseManager> _logger;

        /// <summary>
        /// Gets the connection string used to connecto to the database.
        /// </summary>
        public string ConnectionString { get; private set; }

        public DatabaseManager(IConfiguration configuration, IHttpContextAccessor contextAccessor, ILoggerFactory loggerFactory) {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<DatabaseManager>();

            // Build connection string once
            var dbSection = _configuration.GetSection("Database");
            var host = dbSection["Host"];
            var port = Convert.ToInt32(dbSection["Port"]);
            var username = dbSection["Username"];
            var password = dbSection["Password"];
            var schema = dbSection["Schema"];
            ConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4}",
                host, port, username, password, schema);
        }

        private DataContext _context = null;

        /// <summary>
        /// Gets an open connection to the database.
        /// </summary>
        public DataContext Context {
            get {
                if (_context == null) {
                    _context = new DataContext(ConnectionString, _logger);
                    _contextAccessor.HttpContext.Response.RegisterForDispose(_context);

                    _logger.LogDebug(LoggingEvents.DatabaseConnection, "Database context opened");
                }

                return _context;
            }
        }

    }

}
