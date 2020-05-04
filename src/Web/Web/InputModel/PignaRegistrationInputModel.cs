using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.InputModel {

    public class PignaRegistrationInputModel {

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email di riferimento")]
        [Required(ErrorMessage = "Devi specificare un indirizzo e-mail.")]
        [EmailAddress(ErrorMessage = "Indirizzo e-mail non valido.")]
        public string Email { get; set; }

        [Display(Name = "Codice meccanografico della scuola")]
        [Required(ErrorMessage = "Codice richiesto.")]
        [StringLength(12, MinimumLength = 8, ErrorMessage = "Lunghezza del codice non corretta.")]
        public string MeccanographicCode { get; set; }

        [Display(Name = "Nome della scuola")]
        [Required(ErrorMessage = "Devi specificare il nome della scuola.")]
        public string SchoolName { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Indirizzo")]
        [Required(ErrorMessage = "Devi specificare l'indirizzo della scuola.")]
        [MinLength(10, ErrorMessage = "Indirizzo della scuola troppo corto.")]
        public string SchoolAddress { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "CAP")]
        [Required(ErrorMessage = "Devi specificare il CAP della scuola.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Il CAP deve essere di 5 lettere.")]
        public string SchoolCap { get; set; }

        [Display(Name = "Città")]
        [Required(ErrorMessage = "Devi specificare la città della scuola.")]
        [MinLength(3, ErrorMessage = "Nome della città troppo corto.")]
        public string SchoolCity { get; set; }

        [Display(Name = "Provincia")]
        [Required(ErrorMessage = "Devi specificare la provincia della scuola.")]
        [MinLength(2, ErrorMessage = "Nome della provincia troppo corto.")]
        public string SchoolProvince { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Indirizzo")]
        [Required(ErrorMessage = "Devi specificare il tuo indirizzo di casa.")]
        [MinLength(10, ErrorMessage = "Indirizzo di casa troppo corto.")]
        public string PersonalAddress { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "CAP")]
        [Required(ErrorMessage = "Devi specificare il CAP del tuo indirizzo di casa.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Il CAP deve essere di 5 lettere.")]
        public string PersonalCap { get; set; }

        [Display(Name = "Città")]
        [Required(ErrorMessage = "Devi specificare la città del tuo indirizzo di casa.")]
        [MinLength(3, ErrorMessage = "Nome della città troppo corto.")]
        public string PersonalCity { get; set; }

        [Display(Name = "Provincia")]
        [Required(ErrorMessage = "Devi specificare la provincia del tuo indirizzo di casa.")]
        [MinLength(2, ErrorMessage = "Nome della provincia troppo corto.")]
        public string PersonalProvince { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Numero di telefono")]
        [Phone(ErrorMessage = "Specifica un numero di telefono valido.")]
        public string PhoneNumber { get; set; }

    }

}
