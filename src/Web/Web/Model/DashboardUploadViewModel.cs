using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeMooc.Web.Model {

    public class DashboardUploadViewModel : DashboardBaseViewModel {

        public string ProfilePictureFilename { get; set; }

        public string CurriculumFilename { get; set; }

        public enum CurriculumUploadFailure {
            None,
            WrongExtension
        }

        public CurriculumUploadFailure CurriculumFailure { get; set; }

    }

}
