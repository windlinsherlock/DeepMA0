using PACS.Commons;
using PACS.Commons.Entities;
using PACS.Commons.Events;
using PACS.Extensions;
using PACS.Models;
using PACS.Shared.Entities;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;

namespace PACS.ViewModels
{
    public class MainViewModel : BindableBase, Common.IConfigureService
    {
        private readonly IContainerProvider containerProvider;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly DataContext dataContext;
        private readonly UserConfiguration userConfiguration;

        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    IsMenuOpen = false;
                }
            }
        }

        private bool isMenuOpen;
        public bool IsMenuOpen
        {
            get => isMenuOpen;
            set => SetProperty(ref isMenuOpen, value);
        }

        public MainViewModel(IContainerProvider containerProvider,
            IRegionManager regionManager, IEventAggregator eventAggregator,DataContext dataContext, UserConfiguration userConfiguration)
        {
            MenuItems = new ObservableCollection<NavigatorItem>();
            
            //导航栏跳转，参数是NavigatorItem
            NavigateCommand = new DelegateCommand<NavigatorItem>(Navigate);

            this.containerProvider = containerProvider;
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dataContext = dataContext;
            this.userConfiguration = userConfiguration;

            eventAggregator.GetEvent<MessageEvent>().Subscribe(Navigate, arg => arg.Filter.Equals("Navigate"));

            // 载入已有图片
            

            // 数据库初始化
            this.dataContext.Database.EnsureCreated();
        }

        /// <summary>
        /// 导航菜单项
        /// </summary>
        private ObservableCollection<NavigatorItem> menuItems;

        public ObservableCollection<NavigatorItem> MenuItems
        {
            get { return menuItems; }
            set { menuItems = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 创建导航菜单
        /// </summary>
        void CreateMenuBar()
        {
            //MenuItems.Add(new NavigatorItem() { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuItems.Add(new NavigatorItem() { Icon = "Notebook", Title = "工作簿", NameSpace = "WorkBooksView" });
            MenuItems.Add(new NavigatorItem() { Icon = "NotebookPlus", Title = "标注", NameSpace = "MarkView" });
            MenuItems.Add(new NavigatorItem() { Icon = "Note", Title = "自动诊断", NameSpace = "AutoDiagnoseView" });
            MenuItems.Add(new NavigatorItem() { Icon = "Folder", Title = "自动诊断结果", NameSpace = "AutoDiagnoseFoldersView" });
            MenuItems.Add(new NavigatorItem() { Icon = "Web", Title = "发现", NameSpace = "ZoneView" });
            //MenuItems.Add(new NavigatorItem() { Icon = "Account", Title = "管理员", NameSpace = "AdminView" });
            if (userConfiguration.IsAdmin)
            {
                MenuItems.Add(new NavigatorItem() { Icon = "Account", Title = "管理员", NameSpace = "AdminView" });
            }
            MenuItems.Add(new NavigatorItem() { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });

            SelectedItem = MenuItems[0];
        }

        public DelegateCommand<NavigatorItem> NavigateCommand { get; private set; }
        public DelegateCommand<string> ExecuteCommand { get; private set; }


        //具体跳转到某个view 
        private void Navigate(NavigatorItem obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.NameSpace))
                return;


            //首先IRegionManager接口获取全局定义的可用区域
            //往这个区域动态去设置内容传递的参数
            //设置内容的方式是通过依赖注入的形式
            //PrismManager.MainViewRegionName就是一个字符串"MainView"
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace);
 
        }

        private void Execute(string obj)
        {
            
        }

        private void Navigate(MessageModel obj)
        {
            SelectedItem = MenuItems[int.Parse(obj.Message)];
            Navigate(MenuItems[int.Parse(obj.Message)]);
        }


        /// <summary>
        /// 配置首页初始化参数
        /// </summary>
        public void Configure()
        {           
            CreateMenuBar();
            Navigate(MenuItems[0]);
        }



    }
}
