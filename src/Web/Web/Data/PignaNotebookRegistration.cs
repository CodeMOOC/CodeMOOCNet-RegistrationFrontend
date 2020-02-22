using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMooc.Web.Data {

    [Table("PignaNotebookRegistrations")]
    public class PignaNotebookRegistration {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("RegistrationID")]
        public int RegistrationId { get; set; }

        public string Email { get; set; }

        [Column("MeccanoCode")]
        [MaxLength(16)]
        public string MeccanographicCode { get; set; }

        [MaxLength(256)]
        public string SchoolName { get; set; }

        [MaxLength(512)]
        public string SchoolAddress { get; set; }

        [Column("SchoolCAP")]
        [MaxLength(5)]
        public string SchoolCap { get; set; }

        [MaxLength(64)]
        public string SchoolCity { get; set; }

        [MaxLength(32)]
        public string SchoolProvince { get; set; }

        [Column("Phone")]
        [MaxLength(32)]
        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

    }

}
