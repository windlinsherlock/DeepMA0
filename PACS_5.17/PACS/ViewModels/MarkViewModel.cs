using PACS.Commons.Events;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using PACS.Commons.Models;
using PACS.Commons.Entities;
using System.Linq;
using PACS.Commons;

namespace PACS.ViewModels
{
    class MarkViewModel : BindableBase
    {
        private readonly IContainerProvider containerProvider;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private UserConfiguration userConfiguration;


        
        public String MarkState { get; set; }

        /// <summary>
        /// 待打开的文件
        /// </summary>
        private ObservableCollection<FileItem> fileItems;
        public ObservableCollection<FileItem> FileItems
        {
            get { return fileItems; }
            set { fileItems = value; RaisePropertyChanged(); }
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

        private readonly DataContext _context = new DataContext();

        public MarkViewModel(IEventAggregator eventAggregator, UserConfiguration userConfiguration, DataContext dataContext)
        {
            this.eventAggregator = eventAggregator;
            this._context = dataContext;
            this.userConfiguration = userConfiguration;
            //eventAggregator.GetEvent<MessageEvent>().Subscribe(LoadImage,arg => arg.Filter.Equals("Mark"));

            // 标注候选框
            SelectionChangedCommand = new DelegateCommand<ComboBox>(SelectionChanged);

            // 侧边栏
            FileItems = new ObservableCollection<FileItem>();
            NavigateCommand = new DelegateCommand<FileItem>(Navigate);
            eventAggregator.GetEvent<MessageEvent>().Subscribe(LoadSidebar, arg => arg.Filter.Equals("Sidebar"));
        }



        private void LoadSidebar(MessageModel obj)
        {
            FileItems.Clear();
            FileItems.AddRange(userConfiguration.FileItems);
            FileItems.RemoveAt(FileItems.Count - 1);
            SelectedItem = FileItems.Single(i => i.Id.Equals(obj.Message));
            

            /*var items = _context.FileFolders.Where(f => f.FileFolderId.Equals(obj.Message)).FirstOrDefault();

            foreach (var item in items.FileItems)
            {
                FileItems.Add(new FileItem(item.Path));
            }*/


        }

        /// <summary>
        /// 标注复选框
        /// </summary>
        public DelegateCommand<ComboBox> SelectionChangedCommand { get; private set; }

        private void SelectionChanged(ComboBox box)
        {
            MarkState = box.SelectedItem.ToString().Split(":")[1];
            //System.Windows.MessageBox.Show(MarkState, "Exception Sample",System.Windows.MessageBoxButton.OK);
        }


        /// <summary>
        /// 侧边栏跳转
        /// </summary>
        public DelegateCommand<FileItem> NavigateCommand { get; private set; }
        private void Navigate(FileItem obj)
        {
            if (obj == null)
                return;
            if (obj.Id != null) 
                ImageMark(obj);
            else
            {
                System.Windows.MessageBox.Show("该文件已被移除");
            }

        }

        private void ImageMark(FileItem obj)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Navigate",
                Message = "1",
            });
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Mark",
                Message = obj.Id,
            });
        }

        public DelegateCommand<FileItem> AutoDiagnoseImageCommand { get; private set; }

        private void AutoDiagnoseImage(FileItem obj)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Navigate",
                Message = "2",
            });
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "AutoDiagnose",
                Message = obj.Id,
            });
        }
    }
}
