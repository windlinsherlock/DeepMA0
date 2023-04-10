using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PACS.Shared.Entities
{
    public class AutoDiagnoseItemModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public String AutoDiagnoseItemId { get; set; }

        public String UserId { get; set; }
        public String Name { get; set; }

        public byte[] Image { get; set; }

        public byte[] ThermodynamicChart { get; set; }

        public byte[] LabelImage { get; set; }

        public String DiagnoseContent { get; set; }

        public AutoDiagnoseItemModel()
        {
                
        }

        public AutoDiagnoseItemModel(string userId,string name, byte[] image, byte[] thermodynamicChart, byte[] labelImage, string diagnoseContent)
        {
            this.Name = name;
            this.Image = image;
            this.ThermodynamicChart = thermodynamicChart;
            this.LabelImage = labelImage;   
            this.DiagnoseContent = diagnoseContent; 

        }

    }
    
}
