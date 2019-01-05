using System;

namespace CodeMooc.Web.Data {

    public class Registration {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string Birthplace { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordSchema { get; set; }
        public string PasswordHash { get; set; }
        public bool IsTeacher { get; set; }
        public bool HasAttendedMooc { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
        public string ConfirmationSecret { get; set; }
        public DateTime? ConfirmationTimestamp { get; set; }

    }

}
