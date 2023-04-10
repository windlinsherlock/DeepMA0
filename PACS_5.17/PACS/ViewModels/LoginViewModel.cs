using PACS.Extensions;
using PACS.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RestSharp;
using PACS.Shared.DTOs;
using PACS.Commons;
using PACS.Commons.Entities;
using PACS.Shared.Entities;
using System.Collections.ObjectModel;

namespace PACS.ViewModels
{
    class LoginViewModel : BindableBase, IDialogAware
    {
        public string Title { get; set; } = "骨科图像自动标注";

        public event Action<IDialogResult> RequestClose;

        

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IUserService userService;
        private readonly ICloudService cloudService;
        private IAutoDiagnoseService autoDiagnoseService;
        private readonly DataContext dataContext;
        private UserConfiguration userConfiguration;

        private string userName;
        public String UserName
        {
            get { return userName; }
            set { userName = value; RaisePropertyChanged(); }
        }

        private string password;
        public String Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged(); }
        }


        public LoginViewModel(IEventAggregator eventAggregator, 
            IUserService userService, ICloudService cloudService,IAutoDiagnoseService autoDiagnoseService,
            DataContext dataContext,
            UserConfiguration userConfiguration)
        {
            this.eventAggregator = eventAggregator;
            this.userService = userService;
            this.cloudService = cloudService;
            this.dataContext = dataContext;
            this.userConfiguration = userConfiguration;
            this.autoDiagnoseService = autoDiagnoseService;
            ExecuteCommand = new DelegateCommand<string>(Execute);
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }


        private void Execute(string obj)
        {
            switch (obj)
            {
                case "Login": Login(); break;
                case "LoginOut": LogOut(); break;
                case "Resgiter": Resgiter(); break;
                case "OffLine": OffLine();break;
            }
        }

        

      

        private async void Login()
        {
            
            if (string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(Password))
            {
                return;
            }

            var loginResult = await userService.Login(new LoginDTO { UserName = UserName,Password = Password});
            ;

            if (loginResult != null && loginResult.Status)
            {
                // 获取JWT
                userConfiguration.Token = Newtonsoft.Json.JsonConvert.DeserializeObject<Shared.Contact.TokenResult> ((string)loginResult.Result);
                userConfiguration.UserId = loginResult.Message.Split("/")[0];

                if (loginResult.Message.Split("/")[1].Equals("True"))
                    userConfiguration.IsAdmin = true;

                // 加载用户数据
                var local = dataContext.FileFolders.Where(item => item.AccessModifier.Equals("local")).ToList();

                // 加载本地文件
                userConfiguration.LocalFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>(); ;
                foreach (var item in local)
                {
                    userConfiguration.LocalFileFolders.Add(item);
                }

                // 加载云端文件
                var result = await cloudService.GetFolders();
                userConfiguration.PublicFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>();
                userConfiguration.PrivateFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>();
                if (result.Result != null) 
                {
                    var folders = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>>((string)result.Result);
                    foreach (var folder in folders)
                    {
                        // 如果不存在则下载云端文件夹
                        try
                        {
                            if(folder.AccessModifier.Equals("public"))
                                userConfiguration.PublicFileFolders.Add(folder);
                            else
                                userConfiguration.PrivateFileFolders.Add(folder);

                            var exsit = dataContext.FileFolders.Contains(folder);
                            // 如果不存在则下载云端文件夹
                            if (!exsit)
                                dataContext.FileFolders.Add(folder);
                            // 存在则进行同步
                            else
                            {
                                dataContext.FileFolders.Update(folder);
                            }
                        }
                        catch(Exception ex)
                        {

                        }


                  
                        //var files = dataContext.FolderFiles.Where(i => i.FileFolderId.Equals(folder.FileFolderId)).ToList();

                        /*var items = folder.FileItems;

                        // 获取文件夹下的对应文件

                        

                        foreach (var item in items)
                        {
                            // 如果不存在则下载
                            if (!dataContext.FileItems.Contains(item))
                            {
                                var response = await cloudService.GetFileItem(item.FileItemId);
                                ;
                            }
                        }*/
                    }

                    dataContext.SaveChanges();
                }
                else
                {
                    userConfiguration.PublicFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>();
                }

                //加载自动诊断结果文件
                var response = await autoDiagnoseService.GetAutoDiagnoseFolders();
                if (response.Status)
                {
                    userConfiguration.AutoDiagnoseFolders = Newtonsoft.Json.JsonConvert.DeserializeObject<ObservableCollection<AutoDiagnoseFolderDTO>>((string)response.Result);
                }

                //登录成功，模拟对话框点击确认
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            else
            {

                
                System.Windows.MessageBox.Show(loginResult.Message);
                //登录失败提示...
                //eventAggregator.SendMessage(loginResult.Message, "Login");
            }
        }

        private async void OffLine()
        {
            var local = dataContext.FileFolders.Where(item => item.AccessModifier.Equals("local")).ToList();

            // 加载本地文件
            userConfiguration.LocalFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>(); ;
            foreach (var item in local)
            {
                userConfiguration.LocalFileFolders.Add(item);
            }

            

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
        }


        private void LogOut()
        {
            throw new NotImplementedException();
        }

        private async Task Resgiter()
        {
            if (string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrWhiteSpace(Password))
            {
                return;
            }

            var registerResult = await userService.Register(new LoginDTO { UserName = UserName, Password = Password });

            if (registerResult != null && registerResult.Status)
            {
                // 获取JWT
                userConfiguration.Token = Newtonsoft.Json.JsonConvert.DeserializeObject<Shared.Contact.TokenResult>((string)registerResult.Result);
                userConfiguration.UserId = registerResult.Message.Split("/")[0];

                if (registerResult.Message.Split("/")[1].Equals("True"))
                    userConfiguration.IsAdmin = true;

                // 加载用户数据
                var local = dataContext.FileFolders.Where(item => item.AccessModifier.Equals("local")).ToList();

                // 加载本地文件
                userConfiguration.LocalFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>(); ;
                foreach (var item in local)
                {
                    userConfiguration.LocalFileFolders.Add(item);
                }

                
                
                userConfiguration.PublicFileFolders = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>();
                


            }
            else
            {
                System.Windows.MessageBox.Show((string)registerResult.Result);
                //登录失败提示...
                eventAggregator.SendMessage(registerResult.Message, "Register");
            }
        }
    }
}
