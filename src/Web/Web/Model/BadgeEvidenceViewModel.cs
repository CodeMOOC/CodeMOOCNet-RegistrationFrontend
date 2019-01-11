using System;
using System.Collections.Generic;

namespace CodeMooc.Web.Model {

    public class BadgeEvidenceViewModel {

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public BadgeType BadgeType { get; set; }
        public DateTime IssueTimestamp { get; set; }

    }

}
