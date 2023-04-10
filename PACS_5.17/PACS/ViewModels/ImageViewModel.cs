using MaterialDesignThemes.Wpf;
using PACS.Common;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.ViewModels
{
    public class ImageViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get ; set; }

        public ImageViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public DelegateCommand CancelCommand { get; set; }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
        }

        public DelegateCommand SaveCommand { get; set; }
        

        /// <summary>
        /// 确定
        /// </summary>
        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK
                DialogParameters param = new DialogParameters();

                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            
        }
    }
}
