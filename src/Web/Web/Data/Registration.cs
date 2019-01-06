using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMooc.Web.Data {

    [Table("Registrations")]
    public class Registration {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public string Birthplace { get; set; }
        public string FiscalCode { get; set; }
        public string AddressStreet { get; set; }
        public string AddressCity { get; set; }
        public string AddressCap { get; set; }
        public string AddressCountry { get; set; }

        public string Email { get; set; }
        public string PasswordSchema { get; set; }
        public string PasswordHash { get; set; }

        public string Category { get; set; }

        public bool HasAttendedMooc { get; set; }
        public bool HasCompletedMooc { get; set; }

        public DateTime RegistrationTimestamp { get; set; }

        public string ConfirmationSecret { get; set; }
        public DateTime? ConfirmationTimestamp { get; set; }

    }

}
