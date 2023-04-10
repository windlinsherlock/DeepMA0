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
    /// AutoDiagnoseView.xaml 的交互逻辑
    /// </summary>
    public partial class AutoDiagnoseView : UserControl
    {
        private readonly IEventAggregator eventAggregator;
        public AutoDiagnoseView(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            InitializeComponent();
           
        }
        private void CreateFolder(object sender, RoutedEventArgs e)
        {
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "SaveAutoDiagnoseItem",
                Message = FileFolder.Text,
            });
            FileFolder.Text = "";
        }
    }
}
