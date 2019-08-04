using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Data {

    [Table("Emails")]
    public class Email {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Email")]
        public string Address { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RegistrationId { get; set; }

        [ForeignKey(nameof(RegistrationId))]
        public Registration Registration { get; set; }

        [DefaultValue(false)]
        public bool IsPrimary { get; set; }

        public DateTime AssociationTimestamp { get; set; }

        [InverseProperty(nameof(Donation.Address))]
        public List<Donation> AssociatedDonations { get; set; }

        [InverseProperty(nameof(Badge.Address))]
        public List<Badge> AssociatedBadges { get; set; }

    }

}
