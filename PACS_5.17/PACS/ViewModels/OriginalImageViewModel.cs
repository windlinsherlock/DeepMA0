using PACS.Commons.Entities;
using PACS.Commons;
using PACS.Shared.DTOs;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PACS.ViewModels
{
    class OriginalImageViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "图片";

        public event Action<IDialogResult> RequestClose;

        private BitmapImage image;

        public BitmapImage Image
        {
            get { return image; }
            set { image = value; RaisePropertyChanged(); }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            image = parameters.GetValue<BitmapImage>("bitmapImage");
        }

        public DelegateCommand CloseCommand { get; set; }


        /// <summary>
        /// 确定
        /// </summary>
        private void Close()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private readonly IEventAggregator eventAggregator;
        private UserConfiguration userConfiguration;



        public OriginalImageViewModel(IEventAggregator eventAggregator,
            UserConfiguration userConfiguration)
        {
            this.eventAggregator = eventAggregator;
            this.userConfiguration = userConfiguration;
            CloseCommand = new DelegateCommand(Close);

        }

    }
}
