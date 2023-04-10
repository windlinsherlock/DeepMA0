using PACS.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.Shared.DTOs
{
    public class FileItemDTO
    {
        public String FileItemId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public String Name { get; set; }

        public String Md5 { get; set; }

        public String Thumb { get; set; }

        #region
        public String MaskId { get; set; }

        public int Positive { get; set; }

        public int Negative { get; set; }
        #endregion

        public FileItemDTO()
        {

        }

        public FileItemDTO(FileItemModel file)
        {
            this.FileItemId = file.FileItemId;
            this.Md5 = file.Md5;
            this.Name = file.Name;
            this.Thumb = Newtonsoft.Json.JsonConvert.SerializeObject(file.Thumb);
        }

        public FileItemDTO(FileItemModel file,bool isAdmin)
        {
            this.FileItemId = file.FileItemId;
            this.Name = file.Name;
            this.Positive = file.Positive;
            this.Negative = file.Negative;
            this.MaskId = file.MaskId;
        }
    }
}
