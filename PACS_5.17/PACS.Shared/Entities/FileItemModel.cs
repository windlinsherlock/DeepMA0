using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;


namespace PACS.Shared.Entities
{
    public class FileItemModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public String FileItemId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public String Name { get; set; }

        public String Md5 { get; set; }

        public byte[] Image { get; set; }

        public byte[] Thumb { get; set; }

        public String MaskId { get; set; }

        public int Positive { get; set; }

        public int Negative { get; set; }

        /*/// <summary>
        /// 所属的文件夹
        /// </summary>
        public ICollection<FileFolderModel>
           FileFolders
        { get; set; } =
           new ObservableCollection<FileFolderModel>();

        /// <summary>
        /// 该图片对应的标注
        /// </summary>
        public ICollection<FileMaskModel>
            Masks
        { get; set; } =
            new ObservableCollection<FileMaskModel>();*/

        public FileItemModel()
        {
            /*this.Path = path;
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(file);
                Md5 = System.BitConverter.ToString(hash_byte).Replace("-", "");
            }*/
        }

        // 本地
        public FileItemModel(string path)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                file.CopyTo(stream);
                this.Image = stream.ToArray();

               

                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(stream.ToArray());
                this.Md5 = System.BitConverter.ToString(hash_byte).Replace("-", "");

                

                this.Name = info.Name;

                // 生成缩略图
                this.Thumb = Commons.Util.GetThumb(this.Image);
            }
        }

        /*public FileItemModel(string path, FileFolderModel FileFolder)
        {
            this.FileFolder = FileFolder;
            this.Path = path;
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(file);
                Md5 = System.BitConverter.ToString(hash_byte).Replace("-", "");
            }
        }*/

        // 云端
        public FileItemModel(string name,byte[] file)
        {
            this.Name = name;
            this.Image = file;
            

            // 计算MD5
            System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash_byte = get_md5.ComputeHash(file);
            Md5 = System.BitConverter.ToString(hash_byte).Replace("-", "");

            this.Thumb = Commons.Util.GetThumb(file);
        }

        

        public FileItemModel(string path,string md5,string id)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                file.CopyTo(stream);
                this.Image = stream.ToArray();

                // 生成缩略图
                this.Thumb = Commons.Util.GetThumb(this.Image);

                this.Md5 = md5;
                this.FileItemId = id;
                this.Name = info.Name;
            }
        }
    }
}
