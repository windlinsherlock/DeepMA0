using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PACS.WebAPI.Commons.Areas.Identity.Data
{
    public class UserFileFolders
    {
        [Key]
        public String UserId { get; set; }

        [Key]
        public String FileFolderId { get; set; }
    }
}
