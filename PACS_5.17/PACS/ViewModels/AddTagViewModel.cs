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
    public class AddTagViewModel : BindableBase, IDialogHostAware
    {

        public string DialogHostName { get; set; }


        public string TagName { get; set; }

        public AddTagViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }


        public async void OnDialogOpend(IDialogParameters parameters)
        {
        }


        public DelegateCommand CancelCommand { get; set; }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
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
                param.Add("TagName", TagName);


                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }
    }
}
