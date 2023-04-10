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
using System.Windows.Media;

namespace PACS.ViewModels
{
    public class ColorPickerViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }


        public Color Color { get; set; }

        public ColorPickerViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }


        public async void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Color"))
            {
                Color = parameters.GetValue<Color>("Color");
            }
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
                param.Add("R", Color.R);
                param.Add("G", Color.G);
                param.Add("B", Color.B);


                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }
    }
}
