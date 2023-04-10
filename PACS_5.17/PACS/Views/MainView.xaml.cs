using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PACS.Commons.Entities;
using PACS.Extensions;
using Prism.Events;

namespace PACS.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        DataContext dataContext;
        public MainView(IEventAggregator eventAggregator,DataContext dataContext)
        {
            
            InitializeComponent();
            this.dataContext = dataContext;

            // 注册消息顶部提示
            eventAggregator.ResgiterMessage(arg =>
            {
                MessageLine.MessageQueue.Enqueue(arg.Message);
            });

            // 注册等待界面
            eventAggregator.Resgiter(arg =>
            {
                DialogHost.IsOpen = arg.IsOpen;

                if (DialogHost.IsOpen)
                {
                    DialogHost.DialogContent = new ProgressView();
                }
                    
            });

            //窗口控制
            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };
            btnClose.Click += async (s, e) =>
            {
                
                this.Close();
            };
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };

            ColorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
                else
                    this.WindowState = WindowState.Normal;
            };

            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            /*var result = dataContext.FileFolders.Include(e => e.FileItems).Where(e => !e.AccessModifier.Equals("local")).ToList();
            
            if (result != null) 
            {
                foreach(var item in result)
                {
                    dataContext.FileItems.RemoveRange(item.FileItems);
                }
                dataContext.FileFolders.RemoveRange(result);
                
                dataContext.SaveChanges();
            }*/

        }
    }
}
