using Microsoft.WindowsAPICodePack.Dialogs;
using PACS.Commons.Entities;
using PACS.Commons.Events;
using PACS.Commons.Models;
using PACS.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PACS.Commons;
using PACS.Shared.Entities;
using Prism.Regions;
using PACS.Extensions;
using PACS.Views;
using PACS.Services;
using System.Threading.Tasks;


/// <summary>
/// 5.4 16：12
/// 更改显示方式
/// </summary>
namespace PACS.ViewModels
{
    class WorkBooksViewModel : BindableBase
    {

        private bool fileVisibility;
        public bool FileVisibility
        {
            get { return fileVisibility; }
            set
            {
                fileVisibility = value;
                RaisePropertyChanged();
            }
        }

        

        /// <summary>
        /// 本地文件夹
        /// </summary>
        private ObservableCollection<FileFolderModel> localFileFolders;
        public ObservableCollection<FileFolderModel> LocalFileFolderItems
        {
            get { return localFileFolders; }
            set { localFileFolders = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 云端文件夹
        /// </summary>
        private ObservableCollection<FileFolderModel> publicFileFolders;
        public ObservableCollection<FileFolderModel> PublicFileFolderItems
        {
            get { return publicFileFolders; }
            set { publicFileFolders = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<FileFolderModel> privateFileFolders;
        public ObservableCollection<FileFolderModel> PrivateFileFolderItems
        {
            get { return privateFileFolders; }
            set { privateFileFolders = value; RaisePropertyChanged(); }
        }



        /// <summary>
        /// 前端显示文件夹
        /// </summary>
        private ObservableCollection<FileFolderModel> fileFolders;
        public ObservableCollection<FileFolderModel> FileFolderItems
        {
            get { return fileFolders; }
            set { fileFolders = value; RaisePropertyChanged(); }
        }


        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }

        /// <summary>
        /// 当前显示的Tab
        /// </summary>
        private int _selectedTab;
        public int SelectedTab
        {
            get => _selectedTab;
            set 
            { 
                SetProperty(ref _selectedTab, value); 
                FileVisibility = true;
                if(value == 0)
                {
                    FileFolderItems = LocalFileFolderItems;
                }
                else if(value == 1)
                {
                    FileFolderItems = PublicFileFolderItems;
                }
                else if(value == 2)
                {
                    FileFolderItems = PrivateFileFolderItems;
                }
            }
        }



        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;//获取前端的那个公共区域
        private readonly DataContext _context;
        private UserConfiguration userConfiguration;
        private CloudService cloudService;


        public WorkBooksViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, DataContext dataContext, 
            CloudService cloudService,
            UserConfiguration userConfiguration)
        {
            FileVisibility = true;

            NavigateCommand = new DelegateCommand<FileFolderModel>(Navigate);
            BackCommand = new DelegateCommand<String>(Back);

            // 依赖注入
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            _context = dataContext;
            this.userConfiguration = userConfiguration;

            this.cloudService = cloudService;

            SelectedTab = 0;
           

            // 初始化本地文件夹
            LocalFileFolderItems = userConfiguration.LocalFileFolders;
            PublicFileFolderItems = userConfiguration.PublicFileFolders;
            PrivateFileFolderItems = userConfiguration.PrivateFileFolders;



            eventAggregator.GetEvent<MessageEvent>().Subscribe(NewFileFolder, arg => arg.Filter.Equals("0"));
            eventAggregator.GetEvent<MessageEvent>().Subscribe(NewPublicFileFolder, arg => arg.Filter.Equals("1"));
            eventAggregator.GetEvent<MessageEvent>().Subscribe(NewPrivateFileFolder, arg => arg.Filter.Equals("2"));


            eventAggregator.GetEvent<MessageEvent>().Subscribe(Free, arg => arg.Filter.Equals("Free"));
            eventAggregator.GetEvent<MessageEvent>().Subscribe(Delete, arg => arg.Filter.Equals("Delete"));

            eventAggregator.GetEvent<MessageEvent>().Subscribe(FileFolderRefresh, arg => arg.Filter.Equals("FileFolderRefresh"));
        }

        /// <summary>
        /// 切换文件夹、文件视图显示
        /// </summary>
        public DelegateCommand<String> BackCommand { get; private set; }

        private void Back(String obj)
        {
            FileVisibility = !FileVisibility;
        }


        /// <summary>
        /// 添加本地文件夹
        /// </summary>
        /// <param name="obj"></param>
        private void NewFileFolder(MessageModel obj)
        {
            var item = _context.FileFolders.Add(new FileFolderModel(obj.Message)).Entity;
            LocalFileFolderItems.Add(item);
            
            _context.SaveChanges();
        }

        private async void NewPublicFileFolder(MessageModel obj)
        {
            
            var response = await cloudService.CreateFolder(obj.Message,"public");
            if (response.Result != null)
            {
                var folder = Newtonsoft.Json.JsonConvert.DeserializeObject<Shared.Entities.FileFolderModel>((string)response.Result);
                
                _context.FileFolders.Add(folder);
                _context.SaveChanges();

                userConfiguration.PublicFileFolders.Add(folder);
                ;
            }
        }

        private async void NewPrivateFileFolder(MessageModel obj)
        {
            var response = await cloudService.CreateFolder(obj.Message,"private");
            if (response.Result != null)
            {
                var folder = Newtonsoft.Json.JsonConvert.DeserializeObject<Shared.Entities.FileFolderModel>((string)response.Result);

                _context.FileFolders.Add(folder);
                _context.SaveChanges();

                userConfiguration.PrivateFileFolders.Add(folder);
                ;
            }
        }

        private async void Free(MessageModel obj)
        {
            var FolderId = ((FileFolderModel)SelectedItem).FileFolderId;

            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });
            this._context.FolderFiles.Where(i => i.FileFolderId.Equals(FolderId)).Select(i => i.FileId).ForEachAsync(i => this._context.FileItems.Find(i).Image=null);

            this._context.Database.ExecuteSqlRaw("VACUUM;");
            await this._context.SaveChangesAsync();
            
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
        }

        private async void Delete(MessageModel obj)
        {
            
            var FolderId = ((FileFolderModel)SelectedItem).FileFolderId;

            

            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });

            var response = await cloudService.DeleteFolder(FolderId);
            if (response != null)
            {
                eventAggregator.SendMessage(response.Message);
            }

            var item = this._context.FileFolders.Find(FolderId);
            if (item != null)
            {
                this._context.FileFolders.Remove(item);
            }
            var re = this._context.FolderFiles.Where((i) => i.FileFolderId.Equals(FolderId)).ToList();
            this._context.FolderFiles.RemoveRange(re);
            await this._context.SaveChangesAsync();

            foreach(var fileFolder in userConfiguration.PublicFileFolders)
            {
                if (fileFolder.FileFolderId.Equals(FolderId))
                {
                    userConfiguration.PublicFileFolders.Remove(fileFolder);break;
                }
            }

            foreach (var fileFolder in userConfiguration.PrivateFileFolders)
            {
                if (fileFolder.FileFolderId.Equals(FolderId))
                {
                    userConfiguration.PrivateFileFolders.Remove(fileFolder); break;
                }
            }

            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
        }

        private async void FileFolderRefresh(MessageModel obj)
        {
            switch (obj.Message)
            {
                case "private": PrivateFileFolderItems = userConfiguration.PrivateFileFolders;break;
            }
                

        }

        /// <summary>
        /// 进入选定的文件夹
        /// </summary>
        public DelegateCommand<FileFolderModel> NavigateCommand { get; private set; }       

        private void Navigate(FileFolderModel obj)
        {
            if (obj == null)
                return;
            FileVisibility = !FileVisibility;
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "OpenFileFolder",
                Message = obj.FileFolderId,
            });
            /*
                
            else
            {
                MessageBox.Show("该文件已被移除");
            }*/
            
        }
       
    }
}
