using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Data {

    [Table("Donations")]
    public class Donation {

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime Year { get; set; }
        public ushort Amount { get; set; }

    }

}
