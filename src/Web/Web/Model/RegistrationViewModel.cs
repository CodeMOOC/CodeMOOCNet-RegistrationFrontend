using System;
using System.ComponentModel.DataAnnotations;
using CodeMooc.Web.Attributes;

namespace CodeMooc.Web.Model {

    public class RegistrationViewModel {

        [Required(ErrorMessage = "Il nome deve essere valido")]
        [Display(Name = "Nome", Prompt = "Raffaello")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Il cognome deve essere valido")]
        [Display(Name = "Cognome", Prompt = "Sanzio")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "La data di nascita è richiesta")]
        [DataType(DataType.Date, ErrorMessage = "La data di nascita non è valida")]
        [Display(Name = "Data di nascita")]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Il luogo di nascita è richiesto")]
        [Display(Name = "Luogo di nascita", Prompt = "Urbino (PU)")]
        public string Birthplace { get; set; }

        [Required(ErrorMessage = "Il codice fiscale è richiesto")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Il codice fiscale deve essere composto da 16 caratteri")]
        public string FiscalCode { get; set; }

        [Required(ErrorMessage = "L’indirizzo di domicilio è richiesto")]
        [RegularExpression("^[\\w\\s-'’,]*$", ErrorMessage = "L’indirizzo deve essere inserito nel formato “Nome della via, Numero civico”")]
        [Display(Name = "Indirizzo di domicilio", Prompt = "Via Raffaello, 57")]
        public string AddressStreet { get; set; }

        [Required(ErrorMessage = "La città di domicilio è richiesta")]
        [Display(Name = "Città di domicilio", Prompt = "Urbino (PU)")]
        public string AddressCity { get; set; }

        [Required(ErrorMessage = "Il CAP è richiesto")]
        [RegularExpression("^\\d{5}$", ErrorMessage = "Il CAP deve essere composto da 5 numeri")]
        [Display(Name = "Codice di Avviamento Postale", Prompt = "61029")]
        public string AddressCap { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Inserisci la nazione di domicilio")]
        [MinLength(4, ErrorMessage = "Inserire un nome di paese valido")]
        [Display(Name = "Nazione di domicilio", Prompt = "Italia")]
        public string AddressCountry { get; set; }

        [Required(ErrorMessage = "Inserire un indirizzo e-mail")]
        [EmailAddress(ErrorMessage = "Inserire un indirizzo e-mail valido")]
        [Display(Name = "Indirizzo e-mail", Prompt = "mario.rossi@example.org")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Devi specificare una password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Ripeti la password scelta sopra")]
        [DataType(DataType.Password)]
        [Display(Name = "Password di conferma")]
        [Compare(nameof(Password), ErrorMessage = "La password non coincide")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Devi specificare una categoria")]
        public UserCategory Category { get; set; } = UserCategory.Teacher;

        public bool HasAttendedMooc { get; set; }

        public bool HasCompletedMooc { get; set; }

        [MustBeTrue(ErrorMessage = "Devi accettare ed impegnarti a rispettare lo statuto")]
        public bool AcceptsStatute { get; set; }

        [MustBeTrue(ErrorMessage = "Devi impegnarti a rispettare il decalogo")]
        public bool AcceptsManifesto { get; set; }

        [MustBeTrue(ErrorMessage = "Devi accettare il processo di iscrizione")]
        public bool AcceptsProcess { get; set; }

        [MustBeTrue(ErrorMessage = "Devi accettare l’informativa sulla privacy")]
        public bool AcceptsPrivacy { get; set; }

        [MustBeTrue(ErrorMessage = "Devi autorizzare il trattamento dei dati")]
        public bool AcceptsConditions { get; set; }

    }

}
