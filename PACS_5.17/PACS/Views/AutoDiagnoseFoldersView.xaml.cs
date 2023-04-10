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
    /// AutoDiagnoseFiles.xaml 的交互逻辑
    /// </summary>
    public partial class AutoDiagnoseFoldersView : UserControl
    {
        public AutoDiagnoseFoldersView()
        {
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
    }
}
