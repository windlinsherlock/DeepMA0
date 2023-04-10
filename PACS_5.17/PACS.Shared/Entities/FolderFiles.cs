using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Shared.Entities
{
    public class FolderFiles
    {
        [Key]
        public String FileId { get; set; }

        [Key]
        public String FileFolderId { get; set; }
    }
}
