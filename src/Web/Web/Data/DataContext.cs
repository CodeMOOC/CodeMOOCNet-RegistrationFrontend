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
            /*modelBuilder.Entity<Move>(e => {
                e.HasKey(nameof(Move.Id));
                e.Property(nameof(Move.Id)).HasColumnType("INT UNSIGNED").ValueGeneratedOnAdd();
                e.Property(nameof(Move.AlexaSessionId)).IsRequired();
                e.Property(nameof(Move.AlexaUserId)).IsRequired();
                e.Property(nameof(Move.Coordinates)).HasColumnType("CHAR(2)").IsRequired();
                e.Property(nameof(Move.Direction)).HasColumnType("CHAR(1)").IsRequired(false);
                e.Property(nameof(Move.CreationTime)).IsRequired();
                e.Property(nameof(Move.ReachedOn)).IsRequired(false);
            });*/
        }

        public override void Dispose() {
            _logger.LogTrace("Disposing data context");

            base.Dispose();
        }

        public DbSet<Registration> Registrations { get; set; }

    }

}
