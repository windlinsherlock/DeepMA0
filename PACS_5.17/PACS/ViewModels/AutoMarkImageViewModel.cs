using MaterialDesignThemes.Wpf;
using PACS.Common;
using PACS.Commons;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Media.Imaging;


namespace PACS.ViewModels
{
    class AutoMarkImageViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "图片";

        private string myTitle;

         public string MyTitle
        {
            get { return myTitle; }
            set { myTitle = value; RaisePropertyChanged(); }
        }

        public event Action<IDialogResult> RequestClose;

        private double gridWidth;
        private double gridHeight;

        public double GridWidth
        {
            get { return gridWidth; }
            set { gridWidth = value; RaisePropertyChanged(); }
        }

        public double GridHeight
        {
            get { return gridHeight; }
            set { gridHeight = value; RaisePropertyChanged(); }
        }

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
            Image = parameters.GetValue<BitmapImage>("autoMarkImage");
            MyTitle = parameters.GetValue<string>("imageName")+"的自动标记结果";
            GridWidth = image.Width;
            GridHeight = image.Height;
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



        public AutoMarkImageViewModel(IEventAggregator eventAggregator,
            UserConfiguration userConfiguration)
        {
            this.eventAggregator = eventAggregator;
            this.userConfiguration = userConfiguration;
            CloseCommand=new DelegateCommand(Close);
        }

    }

}

