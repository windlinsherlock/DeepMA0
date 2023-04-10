using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Shared.DTOs
{
    public class AutoDiagnoseFolderDTO
    {
        public String AutoDiagnoseItemId { get; set; }
        public String Name { get; set; }

        public AutoDiagnoseFolderDTO(string autoDiagnoseItemId, string name)
        {
            this.AutoDiagnoseItemId = autoDiagnoseItemId;
            this.Name = name;
        }
    }
}
