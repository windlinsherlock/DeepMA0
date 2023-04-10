using PACS.Shared.DTOs;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PACS.Commons.Models
{
    public class AutoDiagnoseItem: BindableBase
    {
        public string Name { get; set; }

        private byte[] imageBytes;

        public byte[] ImageBytes
        {
            get { return imageBytes; }
            set
            {
                imageBytes = value;
                RaisePropertyChanged();
            }
        }

        private BitmapImage image;
        public BitmapImage Image
        {
            get { return image; }
            set
            {
                image = value;
                RaisePropertyChanged();
            }
        }

        private BitmapImage thermodynamicChart;
        public BitmapImage ThermodynamicChart
        {
            get { return thermodynamicChart; }
            set
            {
                thermodynamicChart = value;
                RaisePropertyChanged();
            }
        }

        public byte[] ThermodynamicChartBytes { get; set; }

        private BitmapImage labelImage;
        public BitmapImage LabelImage
        {
            get { return labelImage; }
            set
            {
                labelImage = value;
                RaisePropertyChanged();
            }
        }
        

        public byte[] LabelImageBytes { get; set; }

        private String diagnoseContent;
        public String DiagnoseContent
        {
            get { return diagnoseContent; }
            set
            {
                diagnoseContent = value;
                RaisePropertyChanged();
            }
        }
        

        public AutoDiagnoseItem() { }

        public AutoDiagnoseItem(byte[] image) 
        {
            this.ImageBytes = image;
            this.Image = ByteArrayToBitmapImage(this.ImageBytes);
        }

        public AutoDiagnoseItem(string path)
        {
            System.IO.FileInfo info = new System.IO.FileInfo(path);
            using (System.IO.FileStream file = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                file.CopyTo(stream);
                this.ImageBytes = stream.ToArray();
                this.Name = info.Name;

                // 生成缩略图
                this.Image = ByteArrayToBitmapImage(this.ImageBytes);

            }
        }
        public void SetAutoDiagnoseResult(byte[] thermodynamicChartBytes, byte[] labelImageBytes, string diagnoseContent)
        {
            this.ThermodynamicChartBytes = thermodynamicChartBytes;
            this.ThermodynamicChart= ByteArrayToBitmapImage(thermodynamicChartBytes);
            this.LabelImageBytes = labelImageBytes;
            this.LabelImage = ByteArrayToBitmapImage(labelImageBytes);
            this.DiagnoseContent = diagnoseContent;
        }

        public BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
            {
                BitmapImage bmp;

                try
                {
                    bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = new MemoryStream(byteArray);
                    bmp.EndInit();
                }
                catch
                {
                    bmp = null;
                }

                return bmp;
            }

        public AutoDiagnoseItemDTO TurnToDTO()
        {
            AutoDiagnoseItemDTO dto = new AutoDiagnoseItemDTO();    
            dto.Name = this.Name;
            dto.Image= Newtonsoft.Json.JsonConvert.SerializeObject(this.ImageBytes);
            dto.ThermodynamicChart = Newtonsoft.Json.JsonConvert.SerializeObject(this.ThermodynamicChartBytes);
            dto.LabelImage = Newtonsoft.Json.JsonConvert.SerializeObject(this.LabelImageBytes);
            dto.DiagnoseContent = this.DiagnoseContent;
            return dto;
        }

        public AutoDiagnoseItem(AutoDiagnoseItemDTO autoDiagnoseItemDTO)
        {
            this.Name = autoDiagnoseItemDTO.Name;
            this.ImageBytes = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(autoDiagnoseItemDTO.Image) ;
            this.Image = ByteArrayToBitmapImage(ImageBytes);
            this.ThermodynamicChartBytes = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(autoDiagnoseItemDTO.ThermodynamicChart);
            this.ThermodynamicChart = ByteArrayToBitmapImage(ThermodynamicChartBytes);
            this.LabelImageBytes = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(autoDiagnoseItemDTO.LabelImage);
            this.LabelImage = ByteArrayToBitmapImage(LabelImageBytes);
            this.DiagnoseContent=autoDiagnoseItemDTO.DiagnoseContent;   
        }
    }
}
