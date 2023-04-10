using Microsoft.WindowsAPICodePack.Dialogs;
using PACS.Commons.Entities;
using PACS.Commons.Models;
using PACS.Services;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using SixLabors.ImageSharp;
using System.Windows.Media.Imaging;
using PACS.Commons.Events;
using PACS.Extensions;
using PACS.Commons;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PACS.ViewModels
{
    public class AutoDiagnoseViewModel : BindableBase
    {
        private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;
        private UserConfiguration userConfiguration;


        public List<String> ExtensionList = new List<String> { ".png", ".jpg", ".jpeg", ".dcm" };

        private AutoDiagnoseItem autoDiagnoseItem;
        public AutoDiagnoseItem AutoDiagnoseItem
        {
            get { return autoDiagnoseItem; }
            set { autoDiagnoseItem = value; RaisePropertyChanged(); }
        }
        public DelegateCommand<BitmapImage> OriginalImageCommand { get; set; }
        public DelegateCommand UploadFileCommand { get; set; }


        private readonly IAutoDiagnoseService autoDiagnoseService;

        private readonly ICloudService cloudService;

        public AutoDiagnoseViewModel(IEventAggregator eventAggregator, IAutoDiagnoseService autoDiagnoseService,
            UserConfiguration userConfiguration, IDialogService dialogService,
            ICloudService cloudService)
        {
            AutoDiagnoseItem = new AutoDiagnoseItem();
            this.autoDiagnoseService = autoDiagnoseService;
            this.cloudService = cloudService;
            this.eventAggregator = eventAggregator;
            this.userConfiguration = userConfiguration;
            UploadFileCommand = new DelegateCommand(UploadFile);
            OriginalImageCommand = new DelegateCommand<BitmapImage>(OriginalImage);
            this.dialogService = dialogService;
            eventAggregator.GetEvent<MessageEvent>().Subscribe(LoadImage, arg => arg.Filter.Equals("AutoDiagnoseImage"));
            eventAggregator.GetEvent<MessageEvent>().Subscribe(SaveAutoDiagnoseItem, arg => arg.Filter.Equals("SaveAutoDiagnoseItem"));
        }


        /// <summary>
        /// 从手动标注界面跳转过来
        /// </summary>
        /// <param name="obj"></param>
        private async void LoadImage(MessageModel obj)
        {
            //根据图片ID，从云端加载图片
            var response = await cloudService.GetFileItem(obj.Message);
            if (response.Status)
            {
                var Image = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>((string)response.Result);
                AutoDiagnoseItem = new AutoDiagnoseItem(Image);
                //自动诊断
                AutoDiagnose(AutoDiagnoseItem.ImageBytes);
            }


        }
        /// <summary>
        /// 上传本地文件
        /// </summary>
        public  void UploadFile()
        {
            if (AutoDiagnoseItem == null)
                return;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = false;//设置为选择文件
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(dialog.FileName);
                if (IsValidate(file.Extension))
                {
                    AutoDiagnoseItem = new AutoDiagnoseItem(file.FullName);
                    AutoDiagnose(AutoDiagnoseItem.ImageBytes);
                }
            }



        }

        /// <summary>
        /// 自动诊断
        /// </summary>
        /// <param name="image"></param>
        public async void AutoDiagnose(byte[] image)
        {
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });
            var response = await autoDiagnoseService.AutoDiagnose(image);
            if (response != null && response.Status)
            {
                var result = (JObject)response.Result;
                string thermodynamicChartStr = result["ThermodynamicChart"].ToString();
                string labelImageStr = result["LabelImage"].ToString();
                string diagnoseContent = result["DiagnoseContent"].ToString();
                string Name = result["Name"].ToString();
                byte[] thermodynamicChartByte = Convert.FromBase64String(thermodynamicChartStr);
                byte[] labelImageByte = Convert.FromBase64String(labelImageStr);
                AutoDiagnoseItem.SetAutoDiagnoseResult(thermodynamicChartByte, labelImageByte, diagnoseContent);
                eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
            }
            else
            {
                eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
                eventAggregator.SendMessage("自动诊断失败");
            }

        }

        /// <summary>
        /// 验证是否为可处理的文件类型
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool IsValidate(String extension)
        {
            String temp = extension.ToLower();
            if (ExtensionList.Contains(extension))
                return true;
            else
                return false;
        }


        /// <summary>
        /// 查看原图
        /// </summary>
        /// <param name="bitmapImage"></param>
        public void OriginalImage(BitmapImage bitmapImage)
        {
            //this.userConfiguration.OriginalImage = bitmapImage;
            DialogParameters keys = new DialogParameters();
            keys.Add("bitmapImage", bitmapImage);
            dialogService.ShowDialog("OriginalImageView", keys, callback => { }, "DialogWindow");
        }

        /// <summary>
        /// 保存结果
        /// </summary>
        public async void SaveAutoDiagnoseItem(MessageModel obj)
        {
            AutoDiagnoseItemDTO autoDiagnoseItemDTO=autoDiagnoseItem.TurnToDTO();
            autoDiagnoseItemDTO.Name = obj.Message;
            var response = await autoDiagnoseService.SaveAutoDiagnoseItem(autoDiagnoseItemDTO);
            eventAggregator.SendMessage(response.Message);
            //重新载入，这里后期要改
            var response1 = await autoDiagnoseService.GetAutoDiagnoseFolders();
            if (response1.Status)
            {
                userConfiguration.AutoDiagnoseFolders = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<AutoDiagnoseFolderDTO>>((string)response1.Result);
            }
        }

    }
}