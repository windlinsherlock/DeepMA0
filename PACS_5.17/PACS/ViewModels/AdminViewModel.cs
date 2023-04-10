using PACS.Common;
using PACS.Commons.Events;
using PACS.Commons.Models;
using PACS.Extensions;
using PACS.Services;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACS.ViewModels
{
    public class AdminViewModel : BindableBase  //继承第三方的类
    {
        private readonly IAdminService adminService;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogHostService dialog;

        private ObservableCollection<RoleDTO> roles;
        public ObservableCollection<RoleDTO> Roles 
        {
            get { return roles; }
            set { roles = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<UserVM> user { get; set; }
        public ObservableCollection<UserVM> Users
        {
            get { return user; }
            set { user = value; 
                //这个是实现数据变更
                RaisePropertyChanged(); }
        }

        private ObservableCollection<FolderVM> folders { get; set; }
        public ObservableCollection<FolderVM> Folders
        {
            get { return folders; }
            set { folders = value; RaisePropertyChanged(); }
        }

        public AdminViewModel(IAdminService adminService, IEventAggregator eventAggregator, IDialogHostService dialog)
        {
            Roles = new ObservableCollection<RoleDTO>();
            Users = new ObservableCollection<UserVM>();
            Folders = new ObservableCollection<FolderVM>();

            this.adminService = adminService;
            this.eventAggregator = eventAggregator;
            this.dialog = dialog;

            //命令绑定refresh方法
            RefreshCommand = new DelegateCommand<string>(Refresh);

            this.eventAggregator.GetEvent<MessageEvent>().Subscribe(Detail, arg => arg.Filter.Equals("Detail"));
            GetAllUserAsync();
        }
        public DelegateCommand<string> RefreshCommand { get; private set; }

        private void Refresh(string obj)
        {
            if (obj == "GetAllUserAsync")
                GetAllUserAsync();
            if (obj == "GetAllFolderAsync")
                GetAllFolderAsync();
        }

        public DelegateCommand<string> DetailCommand { get; private set; }
        private async void Detail(MessageModel obj)
        {
            DialogParameters param = new DialogParameters();
            param.Add("FileItemId", obj.Message);


            var dialogResult = await dialog.ShowDialog("MarkDetailsView", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                string MaskId = dialogResult.Parameters.GetValue<string>("MaskId");

                var response = await adminService.SetItemMask(obj.Message, MaskId);

                if (response != null)
                {
                    if (response.Status)
                    {
                        foreach (var folder in Folders)
                        {
                            foreach (var item in folder.FileItems)
                            {
                                if (item.FileItemId.Equals(obj.Message))
                                    item.MaskId = MaskId;
                            }
                        }
                        RaisePropertyChanged(MaskId);
                        eventAggregator.SendMessage("标注结果更改成功");
                    }

                    else
                        eventAggregator.SendMessage("标注结果更改失败");
                }
            }
        }

        /*private async Task DetailDialog(string id)
        {
            DialogParameters param = new DialogParameters();
            param.Add("FileItemId", id);
            

            var dialogResult = await dialog.ShowDialog("MarkDetailsView", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                string MaskId = dialogResult.Parameters.GetValue<string>("MaskId");

                var response = await adminService.SetItemMask(id, MaskId);

                if(response != null)
                {
                    if (response.Status)
                    {
                        foreach(var folder in Folders)
                        {
                            foreach(var item in folder.FileItems)
                            {
                                if (item.FileItemId.Equals(id))
                                    item.MaskId = MaskId;
                            }
                        }
                        RaisePropertyChanged(MaskId);
                        eventAggregator.SendMessage("标注结果更改成功");
                    }
                        
                    else
                        eventAggregator.SendMessage("标注结果更改失败");
                }
            }
        }*/


        public async Task GetAllRoleAsync()
        {
            var response = await adminService.Role();

            if (response != null) 
            {
                if (response.Status)
                {
                    Roles.AddRange(Newtonsoft.Json.JsonConvert.DeserializeObject <List<RoleDTO>>((string)response.Result));
                }
            }
        }

        public async Task GetAllUserAsync()
        {
            var response = await adminService.User();

            if (response != null)
            {
                if (response.Status)
                {
                    Users.Clear();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserDTO>>((string)response.Result);
                    foreach (var user in result)
                    {
                        
                        var vm = new UserVM { Id = user.Id, Name = user.Name, Email = user.Email, AccessFailedCount = user.AccessFailedCount, EmailConfirmed = user.EmailConfirmed, LockoutEnabled = user.LockoutEnabled };
                        Users.Add(vm);
                    }

                }
            }
        }

        public async Task GetAllFolderAsync()
        {
            var response = await adminService.Folder();

            if (response != null)
            {
                if (response.Status)
                {
                    Folders.Clear();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileFolderModel>>((string)response.Result);
                    foreach (var folder in result)
                    {
                        var folderVM = new FolderVM { FolderId = folder.FileFolderId ,FolderName = folder.Name, AccessModifier = folder.AccessModifier, CreatedBy = folder.CreatedBy };
                        var response2 = await adminService.Folder(folder.FileFolderId);
                        if (response != null)
                        {
                            if (response.Status)
                            {
                                folderVM.FileItems = new List<FileItemDTO>();
                                var files = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileItemDTO>>((string)response2.Result);
                                folderVM.FileItems.AddRange(files);
                            }
                        }
                        Folders.Add(folderVM);
                    }

                }
            }
        }
    }

    public class UserVM
    {
        
        public string Id { get; set; }

        [DisplayName("用户名")]
        public string Name { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }

    public class FolderVM
    {
        public String FolderId { get; set; }

        public String FolderName { get; set; }

        public String AccessModifier { get; set; }

        public String CreatedBy { get; set; }

        public List<FileItemDTO> FileItems { get; set; }
    }
}
