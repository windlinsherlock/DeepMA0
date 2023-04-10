using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Shared.DTOs
{
    public class AutoDiagnoseItemDTO
    {
        public String Name { get; set; }

        public String Image { get; set; }

        public String ThermodynamicChart { get; set; }

        public String LabelImage { get; set; }

        public String DiagnoseContent { get; set; }

        public AutoDiagnoseItemDTO() { }

        public AutoDiagnoseItemDTO(AutoDiagnoseItemModel autoDiagnoseItem)
        {
            this.Name = autoDiagnoseItem.Name;
            this.Image = Newtonsoft.Json.JsonConvert.SerializeObject(autoDiagnoseItem.Image);
            this.ThermodynamicChart = Newtonsoft.Json.JsonConvert.SerializeObject(autoDiagnoseItem.ThermodynamicChart);
            this.LabelImage = Newtonsoft.Json.JsonConvert.SerializeObject( autoDiagnoseItem.LabelImage);  
            this.DiagnoseContent = autoDiagnoseItem.DiagnoseContent;
        }

        public AutoDiagnoseItemModel TurnToModel()
        {
            AutoDiagnoseItemModel autoDiagnoseItemModel = new AutoDiagnoseItemModel();
            autoDiagnoseItemModel.Name = this.Name; 
            autoDiagnoseItemModel.Image= Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(this.Image);
            autoDiagnoseItemModel.ThermodynamicChart = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(this.ThermodynamicChart);
            autoDiagnoseItemModel.LabelImage= Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(this.LabelImage);
            autoDiagnoseItemModel.DiagnoseContent = this.DiagnoseContent;
            return autoDiagnoseItemModel;
        }
    }
}
