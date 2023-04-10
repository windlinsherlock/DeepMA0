using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PACS.Shared.Entities
{
    public class FileMaskModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public String FileMaskId { get; set; }

        public Boolean IsPositive { get; set; }

        public String Content { get; set; }

        public String CreatedBy { get; set; }

        public String FileItemId { get; set; }
    }
}
