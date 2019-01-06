using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySql;

namespace CodeMooc.Web.Data {

    public class DataContext : DbContext {

        public string ConnectionString { get; }

        protected readonly ILogger<DatabaseManager> _logger;

        public DataContext(
            string connectionString,
            ILogger<DatabaseManager> logger
        ) {
            ConnectionString = connectionString;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySQL(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Registration>(e => {
                e.HasKey(nameof(Registration.Id));
                e.Property(nameof(Registration.Id)).ValueGeneratedOnAdd();
                e.Property(nameof(Registration.Category)).HasConversion<string>();
                e.Property(nameof(Registration.HasAttendedMooc)).HasConversion<int>();
                e.Property(nameof(Registration.HasCompletedMooc)).HasConversion<int>();
            });
        }

        public override void Dispose() {
            _logger.LogTrace("Disposing data context");

            base.Dispose();
        }

        public DbSet<Registration> Registrations { get; set; }

    }

}
