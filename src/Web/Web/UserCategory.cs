using System;
using System.ComponentModel.DataAnnotations;

namespace CodeMooc.Web {

    public enum UserCategory {
        [Display(Name = "Insegnante")]
        Teacher,
        [Display(Name = "Dirigente")]
        SchoolPrincipal,
        [Display(Name = "Genitore")]
        Parent,
        [Display(Name = "Studente")]
        Student,
        [Display(Name = "Pensionato")]
        Retiree,
        [Display(Name = "Altro")]
        Other
    }

}
