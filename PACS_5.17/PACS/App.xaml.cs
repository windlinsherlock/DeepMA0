using DryIoc;
using PACS.Common;
using PACS.Commons;
using PACS.Service;
using PACS.Services;
using PACS.ViewModels;
using PACS.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using System.Windows;
using DialogWindow = PACS.Views.DialogWindow;

namespace PACS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();


            //调用登录弹窗，callback接收返回值
            dialog.ShowDialog("LoginView", callback =>
            {
                //登录失败
                if (callback.Result != ButtonResult.OK)
                {
                    //程序正常运行退出？
                    System.Environment.Exit(0);
                    return;
                }

                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                    service.Configure();
                base.OnInitialized();
            });


            /*var service = App.Current.MainWindow.DataContext as Common.IConfigureService;
            if (service != null)
                service.Configure();
            base.OnInitialized();*/
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer().Register<UserConfiguration>(Reuse.Singleton);


            containerRegistry.GetContainer().Register<HttpRestClient>(made: Parameters.Of.Type<string>(serviceKey: "webUrl"));
            containerRegistry.GetContainer().RegisterInstance(@"http://localhost:5000", serviceKey: "webUrl");

            containerRegistry.Register<IUserService, UserService>();
            containerRegistry.Register<ICloudService, CloudService>();
            containerRegistry.Register<IAutoLableService, AutoLableService>();
            containerRegistry.Register<IAdminService, AdminService>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.Register<IAutoDiagnoseService, AutoDiagnoseService>();

            //注入导航
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<IndexView, IndexViewModel>();
            containerRegistry.RegisterForNavigation<WorkBooksView, WorkBooksViewModel>();
            containerRegistry.RegisterForNavigation<MarkView, MarkViewModel>();
            containerRegistry.RegisterForNavigation<ZoneView, ZoneViewModel>();
            containerRegistry.RegisterForNavigation<AdminView, AdminViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<FileExplorerView, FileExplorerViewModel>();
            containerRegistry.RegisterForNavigation<MarkDetailsView, MarkDetailsViewModel>();
            containerRegistry.RegisterForNavigation<ImageView, ImageViewModel>();
            containerRegistry.RegisterForNavigation<TagPanelView, TagPanelViewModel>();
            containerRegistry.RegisterForNavigation<ColorPickerView, ColorPickerViewModel>();
            containerRegistry.RegisterForNavigation<AddTagView, AddTagViewModel>();
            containerRegistry.RegisterForNavigation<AutoDiagnoseView, AutoDiagnoseViewModel>();
            containerRegistry.RegisterForNavigation<AutoDiagnoseItemView, AutoDiagnoseItemViewModel>();
            containerRegistry.RegisterForNavigation<AutoDiagnoseFoldersView, AutoDiagnoseFoldersViewModel>();

            //注入弹窗
            containerRegistry.RegisterDialog<LoginView, LoginViewModel>();
            containerRegistry.RegisterDialog<AutoMarkImageView, AutoMarkImageViewModel>();
            containerRegistry.RegisterDialog<OriginalImageView, OriginalImageViewModel>();

            //自定义弹窗的父窗口
            containerRegistry.RegisterDialogWindow<DialogWindow>("DialogWindow");
        }
    }
}
