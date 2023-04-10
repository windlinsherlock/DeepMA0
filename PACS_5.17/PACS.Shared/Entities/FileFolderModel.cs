using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PACS.Shared.Entities
{
    public class FileFolderModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public String FileFolderId { get; set; }

        public String Name { get; set; }

        public String Path { get; set; }

        public String CreatedBy { get; set; }

        public String AccessModifier { get; set; }
        

        //public ICollection<FileItemModel> FileItems{ get; set; } =new ObservableCollection<FileItemModel>();

        public FileFolderModel(String Name)
        {
            this.Name = Name;
            this.Path = "";
            this.CreatedBy = "local";
            this.AccessModifier = "local";
        }
    }
}
