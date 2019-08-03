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
                e.Property(nameof(Registration.Category)).HasConversion<string>();
                e.Property(nameof(Registration.HasAttendedMooc)).HasConversion<int>();
                e.Property(nameof(Registration.HasCompletedMooc)).HasConversion<int>();
            });

            modelBuilder.Entity<Donation>(e => {
                e.HasKey(nameof(Donation.Email), nameof(Donation.Year));
                e.Property(nameof(Donation.Year)).HasConversion(YearConverter.Create());
            });

            modelBuilder.Entity<Badge>(e => {
                e.HasKey(nameof(Badge.Email), nameof(Badge.Type), nameof(Badge.Year));
                e.Property(nameof(Badge.Type)).HasConversion(BadgeTypeConverter.Create());
                e.Property(nameof(Badge.Year)).HasConversion(YearConverter.Create());
            });
        }

        public override void Dispose() {
            _logger.LogTrace("Disposing data context");

            base.Dispose();
        }

        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Badge> Badges { get; set; }

    }

}
