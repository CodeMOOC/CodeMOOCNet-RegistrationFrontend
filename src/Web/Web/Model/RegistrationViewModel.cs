using System;
using System.ComponentModel.DataAnnotations;

namespace CodeMooc.Web.Model {

    public class RegistrationViewModel {

        [Required]
        [Display(Name = "Nome", Prompt = "Mario")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Cognome", Prompt = "Rossi")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Indirizzo e-mail", Prompt = "mario.rossi@example.org")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data di nascita")]
        public DateTime Birthday { get; set; }

        [Required]
        [Display(Name = "Luogo di nascita")]
        public string Birthplace { get; set; }

        [Phone]
        [Display(Name = "Numero di telefono")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password di conferma")]
        public string PasswordConfirm { get; set; }

        public bool IsTeacher { get; set; }

        public bool HasAttendedMooc { get; set; }

        [Required]
        public bool AcceptsConditions { get; set; }

        [Required]
        public bool AcceptsGdpr { get; set; }

    }

}
