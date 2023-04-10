using PACS.Commons;
using PACS.Commons.Entities;
using PACS.Commons.Events;
using PACS.Commons.Models;
using PACS.Extensions;
using PACS.Services;
using PACS.Shared.DTOs;
using PACS.Shared.Entities;
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

namespace PACS.ViewModels
{
    public class ZoneViewModel : BindableBase, IConfirmNavigationRequest
    {
        /// <summary>
        /// 云端的公开文件夹
        /// </summary>
        private ObservableCollection<FileFolderModel> publicFileFolders;
        public ObservableCollection<FileFolderModel> PublicFileFolders
        {
            get { return publicFileFolders; }
            set { publicFileFolders = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 打开的文件夹中的文件
        /// </summary>
        private ObservableCollection<FileItem> fileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return fileItems; }
            set { fileItems = value; }
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
        /// 文件夹/文件视图显示切换
        /// </summary>
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; RaisePropertyChanged(); }
        }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly DataContext dataContext;
        private UserConfiguration userConfiguration;
        private CloudService cloudService;

        public ZoneViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, DataContext dataContext,
            CloudService cloudService,
            UserConfiguration userConfiguration)
        {
            isVisible = true;
            FileItems = new ObservableCollection<FileItem>();

            // 依赖注入
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dataContext = dataContext;
            this.userConfiguration = userConfiguration;

            this.cloudService = cloudService;

            LoadPublicFolder();

            BackCommand = new DelegateCommand<String>(Back);
            NavigateCommand = new DelegateCommand<FileFolderModel>(Navigate);

            eventAggregator.GetEvent<MessageEvent>().Subscribe(Save, arg => arg.Filter.Equals("Save"));
        }

        

        /// <summary>
        /// 将公共文件夹保存至我的文件
        /// </summary>
        /// <param name="obj"></param>
        private async void Save(MessageModel obj)
        {
            // 弹出等待动画
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });

            Shared.Contact.ApiResponse response = null;

            if (obj.Message == null)
                response = await cloudService.CopyFolder(((FileFolderModel)SelectedItem).FileFolderId,false);
            else
                response = await cloudService.CopyFolder(((FileFolderModel)SelectedItem).FileFolderId,true);

            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });

            if (response.Status)
            {
                var folder = Newtonsoft.Json.JsonConvert.DeserializeObject<FileFolderModel>((string)response.Result);

                var exist = dataContext.FileFolders.Find(folder.FileFolderId);

                if(exist == null)
                {
                    userConfiguration.PrivateFileFolders.Add(folder);
                    var newFolder = dataContext.FileFolders.Add(folder).Entity;

                    await dataContext.SaveChangesAsync();

                    #region
                    // 获取文件夹下的对应文件
                    response = await cloudService.GetFiles(newFolder.FileFolderId);
                    if (response.Result != null)
                    {
                        // 将DTO转为model
                        var folderfile = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileItemDTO>>((string)response.Result);




                        var fileID = folderfile.Select(i => i.FileItemId).ToList();

                        // 加入文件
                        try
                        {
                            var FilesMD5 = folderfile.Select(x => x.Md5).ToList();
                            var existedFile = dataContext.FileItems.Where(i => FilesMD5.Contains(i.Md5)).Select(i => i.Md5).ToList();
                            if (existedFile.Any())
                            {
                                folderfile = folderfile.Where(i => !existedFile.Contains(i.Md5)).ToList();
                            }

                            var files = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileItemModel>();

                            foreach (var item in folderfile)
                            {
                                files.Add(new FileItemModel { FileItemId = item.FileItemId, Name = item.Name, Md5 = item.Md5, Thumb = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(item.Thumb) });

                            }

                            await dataContext.FileItems.AddRangeAsync(files);
                        }
                        catch (Exception ex)
                        {
                            ;
                        }


                        // 加入文件-文件夹关系
                        try
                        {
                            var relation = new List<FolderFiles>();
                            var existedRelation = dataContext.FolderFiles.Where(i => i.FileFolderId.Equals(newFolder.FileFolderId)).ToList();

                            // 去重
                            if (existedRelation.Any())
                            {
                                foreach (var item in existedRelation)
                                {
                                    fileID.Remove(item.FileId);
                                }
                            }

                            foreach (var item in fileID)
                            {
                                relation.Add(new FolderFiles { FileFolderId = newFolder.FileFolderId, FileId = item });
                            }

                            if (relation.Any())
                            {
                                dataContext.FolderFiles.AddRange(relation);
                                await dataContext.SaveChangesAsync();
                            }

                        }
                        catch (Exception ex)
                        {

                        }

                        /*eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
                        {
                            Filter = "FileFolderRefresh",
                            Message = "private",
                        });*/
                    }
                    #endregion
                }

                else
                {



                    foreach(var item in userConfiguration.PublicFileFolders)
                    {
                        if (item.FileFolderId.Equals(folder.FileFolderId))
                            return;
                    }
                    userConfiguration.PublicFileFolders.Add(folder);
                }

            }
            ;
        }

        /// <summary>
        /// 切换文件夹、文件视图显示
        /// </summary>
        public DelegateCommand<String> BackCommand { get; private set; }

        private void Back(String obj)
        {
            IsVisible = !IsVisible;
        }

        /// <summary>
        /// 进入选定的文件夹
        /// </summary>
        public DelegateCommand<FileFolderModel> NavigateCommand { get; private set; }

        private async void Navigate(FileFolderModel obj)
        {
            if (obj == null)
                return;
            IsVisible = !IsVisible;

            FileItems.Clear();

            
            await LoadFiles(obj.FileFolderId);

        }

        /// <summary>
        /// 加载所有的公共文件夹
        /// </summary>
        /// <returns></returns>
        private async Task LoadPublicFolder()
        {
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });
            var response = await cloudService.GetPublicFolders();
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });

            if (response.Status)
            {
                PublicFileFolders = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileFolderModel>>((string)response.Result);
            }
        }

        /// <summary>
        /// 加载点击的文件夹内的文件
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        private async Task LoadFiles(string folderId)
        {
            var response = await cloudService.GetPublicFiles(folderId);

            if (response.Result != null)
            {
                // 将DTO转为model
                var folderfile = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileItemDTO>>((string)response.Result);

                foreach (var file in folderfile)
                {
                    FileItems.Add(new FileItem(Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(file.Thumb),file.Name));
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadPublicFolder();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        // 用于拦截导航请求
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            bool result = true;
            continuationCallback(result);
        }
    }
}
