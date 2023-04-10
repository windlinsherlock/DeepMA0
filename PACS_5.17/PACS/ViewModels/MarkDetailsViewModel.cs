using MaterialDesignThemes.Wpf;
using PACS.Common;
using PACS.Services;
using PACS.Shared.Entities;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.ViewModels
{
    public class MarkDetailsViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
       

        public string FileItemId { get; set; }

        private readonly IAdminService adminService;

        private ObservableCollection<FileMaskModel> masks { get; set; }
        public ObservableCollection<FileMaskModel> Masks
        {
            get { return masks; }
            set { masks = value; RaisePropertyChanged(); }
        }

        private object? selectedItem;
        public object? SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
            }
        }

        public MarkDetailsViewModel(IAdminService adminService)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);

            Masks = new ObservableCollection<FileMaskModel>();
            this.adminService = adminService;
        }

        public async void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("FileItemId"))
            {
                FileItemId = parameters.GetValue<string>("FileItemId");
            }

            var response = await adminService.Mask(FileItemId);

            if (response != null)
            {
                if (response.Status)
                {
                    if (response.Result != null)
                        Masks.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileMaskModel>>((string)response.Result));
                }
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
            if (SelectedItem == null)
            {
                if (DialogHost.IsDialogOpen(DialogHostName))
                    DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No));
            }

            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK
                DialogParameters param = new DialogParameters();

                var item = (FileMaskModel)SelectedItem;

                param.Add("MaskId", item.FileMaskId);

                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }
    }
}
