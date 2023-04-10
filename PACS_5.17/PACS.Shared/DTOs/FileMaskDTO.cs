using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Shared.DTOs
{
    public class FileMaskDTO
    {
        public string FileMaskId { get; set; }

        public string FileItemId { get; set; }

        public bool IsPositive { get; set; }

        public string Content { get; set; }

        /*public FileMaskDTO(FileMaskModel model)
        {
            this.FileMaskId = model.FileMaskId;
            this.FileItemId = model.FileItemId;
            this.IsPositive = model.IsPositive;
            if(model.Content == null)
            {
                this.Content = " ";
            }
            
        }*/
    }
}
