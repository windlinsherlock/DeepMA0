using PACS.Commons.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PACS.Views
{
    /// <summary>
    /// AdminView.xaml 的交互逻辑
    /// </summary>
    public partial class AdminView : UserControl
    {
        private readonly IEventAggregator eventAggregator;
        public AdminView(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            ScrollViewer viewer = (ScrollViewer)sender;
            viewer.RaiseEvent(eventArg);
        }

        private void Detail(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Detail",
                Message = button.Tag.ToString()
            });
        }
    }
}
