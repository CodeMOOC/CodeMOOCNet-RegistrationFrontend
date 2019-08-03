using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMooc.Web.Data {

    [Table("Registrations")]
    public class Registration {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        [InverseProperty(nameof(Email.Registration))]
        public List<Email> Emails { get; set; }

        public string PasswordSchema { get; set; }
        public string PasswordHash { get; set; }

        public UserCategory Category { get; set; }

        public bool HasAttendedMooc { get; set; }
        public bool HasCompletedMooc { get; set; }

        public DateTime RegistrationTimestamp { get; set; }

        public string ConfirmationSecret { get; set; }
        public DateTime? ConfirmationTimestamp { get; set; }

    }

}
