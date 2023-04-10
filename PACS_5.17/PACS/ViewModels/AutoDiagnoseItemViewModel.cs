using PACS.Commons.Entities;
using PACS.Commons.Events;
using PACS.Commons;
using PACS.Commons.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PACS.Services;
using PACS.Extensions;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using Prism.Services.Dialogs;

namespace PACS.ViewModels
{
    
    class AutoDiagnoseItemViewModel:BindableBase
    {
        private AutoDiagnoseItem autoDiagnoseItem;
        public AutoDiagnoseItem AutoDiagnoseItem
        {
            get { return autoDiagnoseItem; }
            set { autoDiagnoseItem = value; RaisePropertyChanged(); }
        }
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly DataContext datacontext;

        private readonly IAutoDiagnoseService autoDiagnoseService;

        private UserConfiguration userConfiguration;
        private readonly IDialogService dialogService;
        public DelegateCommand<BitmapImage> OriginalImageCommand { get; set; }
        public AutoDiagnoseItemViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, DataContext dataContext,
            IAutoDiagnoseService autoDiagnoseService, IDialogService dialogService,
            UserConfiguration userConfiguration)

        {

            // 依赖注入
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            this.autoDiagnoseService = autoDiagnoseService;

            this.datacontext = dataContext;
            this.userConfiguration = userConfiguration;
            this.dialogService = dialogService;
            OriginalImageCommand = new DelegateCommand<BitmapImage>(OriginalImage);
            eventAggregator.GetEvent<MessageEvent>().Subscribe(GetAutoDiagnoseItem, arg => arg.Filter.Equals("OpenAutoDiagnoseFile"));

        }

        public async void GetAutoDiagnoseItem(MessageModel obj)
        {
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });
            var response = await autoDiagnoseService.GetAutoDiagnoseItem(obj.Message);
            if (response.Result != null)
            {
              
                var autoDiagnoseItemDTO = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoDiagnoseItemDTO>((string)response.Result);
                this.AutoDiagnoseItem = new AutoDiagnoseItem(autoDiagnoseItemDTO);
            }
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
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
    }
}
