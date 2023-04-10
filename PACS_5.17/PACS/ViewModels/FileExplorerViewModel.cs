using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows;

namespace PACS.ViewModels
{
    class FileExplorerViewModel : BindableBase
    {
        public List<String> ExtensionList = new List<String> { ".png", ".jpg", ".jpeg", ".dcm" };

        

        public FileItem AddButton = new FileItem();

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
        /// 待打开的文件
        /// </summary>
        private ObservableCollection<FileItem> fileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return fileItems; }
            set { fileItems = value; }
        }

        /// <summary>
        /// 待打开文件所属的文件夹
        /// </summary>
        public FileFolderModel FileFolder;

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly DataContext datacontext;

        private readonly ICloudService cloudService;

        private UserConfiguration userConfiguration;

        public FileExplorerViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, DataContext dataContext,
            ICloudService cloudService,
            UserConfiguration userConfiguration)
        {
            FileItems = new ObservableCollection<FileItem>();
            NavigateCommand = new DelegateCommand<FileItem>(Navigate);

            // 依赖注入
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            this.cloudService = cloudService;

            this.datacontext = dataContext;
            this.userConfiguration = userConfiguration;

            eventAggregator.GetEvent<MessageEvent>().Subscribe(OpenFileFolder, arg => arg.Filter.Equals("OpenFileFolder"));
            //eventAggregator.GetEvent<MessageEvent>().Subscribe(UpdateFileExists, arg => arg.Filter.Equals("Download"));

            FileItems.Add(AddButton);

        }

        /// <summary>
        /// 接收通讯，打开传入的文件夹
        /// </summary>
        /// <param name="obj"></param>
        private async void OpenFileFolder(MessageModel obj)
        {

            FileItems.Clear();
            GC.Collect();

            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });
            // 获取文件夹下的对应文件概览
            var response = await cloudService.GetFiles(obj.Message);
            if (response.Result != null)
            {
                // 将DTO转为model
                var folderfile = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileItemDTO>>((string)response.Result);

                var fileID = folderfile.Select(i => i.FileItemId).ToList();

                // 加入文件
                try
                {
                    var FilesMD5 = folderfile.Select(x => x.Md5).ToList();
                    var existedFile = datacontext.FileItems.Where(i => FilesMD5.Contains(i.Md5)).Select(i => i.Md5).ToList();
                    if (existedFile.Any())
                    {
                        folderfile = folderfile.Where(i => !existedFile.Contains(i.Md5)).ToList();
                    }

                    var files = new System.Collections.ObjectModel.ObservableCollection<Shared.Entities.FileItemModel>();

                    foreach (var item in folderfile)
                    {
                        files.Add(new FileItemModel { FileItemId = item.FileItemId, Name = item.Name, Md5 = item.Md5, Thumb = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(item.Thumb) });

                    }

                    datacontext.FileItems.AddRange(files);
                }
                catch (Exception ex)
                {
                    ;
                }


                // 加入文件-文件夹关系
                try
                {
                    var relation = new List<FolderFiles>();
                    var existedRelation = datacontext.FolderFiles.Where(i => i.FileFolderId.Equals(obj.Message)).ToList();

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
                        relation.Add(new FolderFiles { FileFolderId = obj.Message, FileId = item });
                    }

                    if (relation.Any())
                        datacontext.FolderFiles.AddRange(relation);
                }
                catch (Exception ex)
                {

                }
                datacontext.SaveChanges();
            }
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
            FileFolder = datacontext.FileFolders.Single(item => item.FileFolderId.Equals(obj.Message));
            if (FileFolder != null)
            {
                /* // 导入文件夹下的图片
                 var items = FileFolder.FileItems;
                 if (items != null)
                 {
                     foreach (var item in items)
                         FileItems.Add(new FileItem(item.Thumb, item.Name,item.FileItemId));

                 }*/

                // 导入文件夹下的图片
                var files = datacontext.FolderFiles.Where(i => i.FileFolderId.Equals(FileFolder.FileFolderId)).ToList();
                if (files.Any())
                {
                    List<string> items = new List<string>();
                    foreach (var item in files)
                        items.Add(item.FileId);

                    var Files = datacontext.FileItems.Where(i => items.Contains(i.FileItemId)).ToList();

                    foreach (var item in Files)
                    {
                        if (item.Image == null)
                        {
                            FileItems.Add(new FileItem(item.Thumb, item.Name, item.FileItemId, true));
                        }
                        else
                        {
                            FileItems.Add(new FileItem(item.Thumb, item.Name, item.FileItemId, false));
                        }

                    }
                }

                if (FileFolder.CreatedBy == userConfiguration.UserId)
                    FileItems.Add(AddButton);

            }
        }

        /*private void UpdateFileExists(MessageModel obj)
        { 
            foreach(var item in FileItems)
            {
                if (item.Id.Equals(obj.Message))
                {
                    item.IsCloud = false;
                    
                    break;
                }
            }
        }*/


        /// <summary>
        /// 打开选中的文件
        /// </summary>
        public DelegateCommand<FileItem> NavigateCommand { get; private set; }

        private void Navigate(FileItem obj)
        {
            if (obj == null)
                return;
            if (obj.IsIcon)
            {
                Plus();
            }
            else
            {
                if (obj.Thumb != null) 
                    ImageMark(obj);
                else
                {
                    MessageBox.Show("该文件已被移除");
                }
            }
        }

        private async void Plus()
        {
            /*CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;//设置为选择文件夹


            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                System.IO.FileInfo[] files = new System.IO.DirectoryInfo(dialog.FileName).GetFiles();
                foreach (System.IO.FileInfo f in files)
                {
                    if (IsValidate(f.Extension))
                    {
                        if (!UserConfiguration.FileList.Contains(f.FullName))
                        {
                            UserConfiguration.FileList.Add(f.FullName);
                            var item = new FileItemModel(f.FullName);

                            FileFolder.FileItems.Add(item);
                            datacontext.FileItems.Add(item);
                            

                            FileItems.Add(new FileItem(f.FullName));
                        }
                    }
                }

                datacontext.SaveChanges();
                FileItems.Remove(AddButton);
                FileItems.Add(AddButton);
            }*/

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;//设置为选择文件夹


            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                // 如果是云端
                if(FileFolder.AccessModifier == "public")
                {
                    System.IO.FileInfo[] files = new System.IO.DirectoryInfo(dialog.FileName).GetFiles();
                    List<string> FileList = new List<string>();
                    foreach (System.IO.FileInfo f in files)
                    {
                        
                        if (IsValidate(f.Extension))
                        {
                            
                            // 每次上传一个文件
                            List<string> temp = new List<string>();
                            temp.Add(f.FullName);

                            var response = await cloudService.UploadFile(temp, FileFolder.FileFolderId);
                            if (response != null && response.Status)
                            {
                                List<FileItemDTO> dtos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FileItemDTO>>((string)response.Result);
                                foreach (FileItemDTO dto in dtos)
                                {
                                    FileItemModel model = new FileItemModel(f.FullName, dto.Md5, dto.FileItemId);


                                    try
                                    {
                                        var result = datacontext.FileItems.Single(i => i.Md5 == model.Md5);
                                        // 该文件已存在
                                        if (result != null)
                                        {
                                            try
                                            {
                                                datacontext.FolderFiles.Add(new FolderFiles { FileId = result.FileItemId, FileFolderId = FileFolder.FileFolderId });
                                            }
                                            catch // 文件夹中已存在该文件
                                            {
                                                break;
                                            }
                                            FileItems.Add(new FileItem(result.Thumb, result.Name, result.FileItemId));
                                        }
                                    }
                                    catch (Exception ex) // 该文件不存在
                                    {
                                        datacontext.FileItems.Add(model);

                                        datacontext.FolderFiles.Add(new FolderFiles { FileId = model.FileItemId, FileFolderId = FileFolder.FileFolderId });

                                        FileItems.Add(new FileItem(model.Thumb, model.Name, model.FileItemId));


                                    }
                                }
                                

                                
                            }
                            
                        }
                            
                            
                    }

                    // 上传多个文件
                    //var response = await cloudService.UploadFile(FileList, FileFolder.FileFolderId);

                    /*var content = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.ObjectModel.ObservableCollection<Shared.DTOs.FileItemDTO>>(response.Result);*/

                    /*foreach (var item in content)
                    {
                        FileItemModel itemModel = new FileItemModel();
                        itemModel.Md5 = item.Md5;
                        itemModel.FileItemId = item.FileItemId;
                        itemModel.Name = item.Name;

                        using (System.IO.FileStream file = new System.IO.FileStream(f.FullName, System.IO.FileMode.Open))
                        {
                            System.IO.MemoryStream stream = new System.IO.MemoryStream();

                            file.CopyTo(stream);

                            itemModel.Image = stream.ToArray();
                            itemModel.Thumb = PACS.Shared.Commons.Util.GetThumb(itemModel.Image);

                        }

                        var entity = datacontext.FileItems.Add(itemModel).Entity;

                        FileItems.Add(new FileItem(itemModel.Thumb, itemModel.Name, entity.FileItemId));
                    }*/
                }

                // 如果是本地文件
                else if(FileFolder.AccessModifier == "local")
                {
                    System.IO.FileInfo[] files = new System.IO.DirectoryInfo(dialog.FileName).GetFiles();
                    foreach (System.IO.FileInfo f in files)
                    {
                        if (IsValidate(f.Extension))
                        {

                            var item = new FileItemModel(f.FullName);
                            try
                            {
                                var result = datacontext.FileItems.Single(i => i.Md5 == item.Md5);
                                // 该文件已存在
                                if (result != null)
                                {
                                    try // 则直接在该文件夹加入
                                    {
                                        datacontext.FolderFiles.Add(new FolderFiles { FileId = result.FileItemId, FileFolderId = FileFolder.FileFolderId });
                                    }
                                    catch // 文件夹中已存在该文件
                                    {
                                        break;
                                    }
                                    FileItems.Add(new FileItem(result.Thumb, result.Name, result.FileItemId));
                                }
                            }
                            catch (Exception ex) // 该文件不存在
                            {
                                var entity = datacontext.FileItems.Add(item).Entity;
                                
                                datacontext.FolderFiles.Add(new FolderFiles { FileId = entity.FileItemId, FileFolderId = FileFolder.FileFolderId });

                                FileItems.Add(new FileItem(item.Thumb, item.Name, entity.FileItemId));

                            }

                            /*if (!UserConfiguration.FileList.Contains(f.FullName))
                            {

                                *//*var item = new FileItemModel(f.FullName);
                                item.FileFolders.Add(FileFolder);

                                var entity = datacontext.FileItems.Add(item).Entity;
                                //FileFolder.FileItems.Add(entity);


                                FileItems.Add(new FileItem(item.Thumb, item.Name, entity.FileItemId));*//*

                                var item = new FileItemModel(f.FullName);
                                try
                                {
                                    var result = datacontext.FileItems.Single(i => i.Md5 == item.Md5);
                                    if(result != null)
                                    {
                                        datacontext.FolderFiles.Add(new FolderFiles { FileId = entity.FileItemId, FileFolderId = FileFolder.FileFolderId });

                                        FileItems.Add(new FileItem(item.Thumb, item.Name, entity.FileItemId));
                                    }
                                }
                                catch(Exception ex)
                                {

                                }
                                

                                var entity = datacontext.FileItems.Add(item).Entity;
                                //FileFolder.FileItems.Add(entity);
                                datacontext.FolderFiles.Add(new FolderFiles { FileId = entity.FileItemId, FileFolderId = FileFolder.FileFolderId });

                                FileItems.Add(new FileItem(item.Thumb, item.Name, entity.FileItemId));
                            }*/
                        }
                    }

                    /*var item = new FileItemModel(dialog.FileName);
                    item.FileFolders.Add(FileFolder);
                    
                    var entity = datacontext.FileItems.Add(item).Entity;
                    //FileFolder.FileItems.Add(entity);

                    
                    FileItems.Add(new FileItem(item.Image, item.Name, entity.FileItemId));*/
                }
                

                datacontext.SaveChanges();                

                FileItems.Remove(AddButton);
                FileItems.Add(AddButton);               
            }
        }

        /// <summary>
        /// 转入标注视图
        /// </summary>
        /// <param name="obj"></param>
        private void ImageMark(FileItem obj)
        {
            userConfiguration.FileItems = this.FileItems;

            // 切换视图
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Navigate",
                Message = "1",
            });

            // 传入文件
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Mark",
                Message = obj.Id,

            });
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Sidebar",
                Message = obj.Id
            });
            
        }


        /// <summary>
        /// 验证是否为可处理的文件类型
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public bool IsValidate(String extension)
        {
            String temp = extension.ToLower();
            if (ExtensionList.Contains(extension))
                return true;
            else
                return false;
        }
    }
}
