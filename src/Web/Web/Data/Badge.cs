using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Data {

    [Table("Badges")]
    public class Badge {

        public string Email { get; set; }

        public BadgeType Type { get; set; }

        public DateTime Year { get; set; }

        public DateTime IssueTimestamp { get; set; }

        public string EvidenceToken { get; set; }

        [ForeignKey(nameof(Email))]
        public Email Address { get; set; }

    }

}
