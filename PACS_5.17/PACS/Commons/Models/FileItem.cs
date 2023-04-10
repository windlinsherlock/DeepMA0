using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes;
using System.Drawing;
using System.IO;

namespace PACS.Commons.Models
{
    public class FileItem : BindableBase
    {
        public String Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public String FileName { get; set; }

        public bool IsIcon { get; set; }

        private bool isCloud;
        public bool IsCloud 
        {
            get { return isCloud; }
            set
            {
                isCloud = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 缩略图
        /// </summary>
        public System.Windows.Media.Imaging.BitmapImage Thumb { get; set; }

        public FileItem()
        {
            this.IsIcon = true;
            this.IsCloud = false;
        }    


        public FileItem(byte[] file,string fileName)
        {

            this.IsIcon = false;

            System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();

            MemoryStream ms = new MemoryStream(file);

            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();

            this.Thumb = image;

            this.FileName = fileName;
        }

        public FileItem(byte[] file, string fileName, string Id)
        {

            this.Id = Id;
            this.IsIcon = false;

            System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();

            MemoryStream ms = new MemoryStream(file);
            
                image.BeginInit();
                image.StreamSource = ms;
                image.EndInit();
            
            this.Thumb = image;

            /*MemoryStream ms = new MemoryStream(file);

            Image image = System.Drawing.Image.FromStream(ms);

            int width=0, height=0;
            if (image.Width > image.Height)
            {
                width = 150;
                height = 150 * image.Height / image.Width;
            }
            else
            {
                height = 150;
                width = 150 * image.Width / image.Height;
            }

            Image.GetThumbnailImageAbort gia = null;
            image = image.GetThumbnailImage(width, height, gia, new System.IntPtr());
            image.Save(fileName);*/

            /*MemoryStream mostream = new MemoryStream();
            Bitmap bmp = new Bitmap(image);
            bmp.Save(mostream, System.Drawing.Imaging.ImageFormat.Jpeg);//将图像以指定的格式存入缓存内存流


            this.Thumb = new System.Windows.Media.Imaging.BitmapImage();
            this.Thumb.BeginInit();
            this.Thumb.StreamSource = mostream;
            this.Thumb.EndInit();*/


            this.FileName = fileName;
        }

        public FileItem(byte[] file, string fileName, string Id, bool IsCloud)
        {

            this.Id = Id;
            this.IsIcon = false;

            System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage();

            MemoryStream ms = new MemoryStream(file);

            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();

            this.Thumb = image;


            this.FileName = fileName;
            this.IsCloud = IsCloud;
        }
    }
}