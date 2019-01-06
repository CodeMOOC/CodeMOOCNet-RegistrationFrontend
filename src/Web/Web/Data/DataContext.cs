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
                e.Property(nameof(Registration.Id)).HasColumnType("INT UNSIGNED").ValueGeneratedOnAdd();
                e.Property(nameof(Registration.Name)).IsRequired();
                e.Property(nameof(Registration.Surname)).IsRequired();
                e.Property(nameof(Registration.Email)).IsRequired();
                e.Property(nameof(Registration.Birthplace)).IsRequired();
                e.Property(nameof(Registration.PasswordSchema)).IsRequired();
                e.Property(nameof(Registration.PasswordHash)).IsRequired();
                e.Property(nameof(Registration.ConfirmationSecret)).IsRequired();
                e.Property(nameof(Registration.IsTeacher)).IsRequired().HasConversion<int>();
                e.Property(nameof(Registration.HasAttendedMooc)).IsRequired().HasConversion<int>();
            });
        }

        public override void Dispose() {
            _logger.LogTrace("Disposing data context");

            base.Dispose();
        }

        public DbSet<Registration> Registrations { get; set; }

    }

}
