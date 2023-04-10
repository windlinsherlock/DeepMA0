using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Events;
using Prism.Regions;
using System.Collections;
using PACS.Commons.Events;

namespace PACS.Views
{
    /// <summary>
    /// WorkBooksView.xaml 的交互逻辑
    /// </summary>
    public partial class WorkBooksView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        public WorkBooksView(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            InitializeComponent();           
        }

        /// <summary>
        /// 配置ScrollViewer滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            ScrollViewer viewer = (ScrollViewer)sender;
            viewer.RaiseEvent(eventArg);
        }

        /// <summary>
        /// 获取用户输入的文件夹名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateFolder(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = Tab.SelectedIndex.ToString(),
                Message = FileFolder.Text,
            });
            FileFolder.Text = "";
        }

        private void FreeCommand(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Free",
            });
        }

        private void DeleteCommand(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Delete",
            });
        }
    }
}
