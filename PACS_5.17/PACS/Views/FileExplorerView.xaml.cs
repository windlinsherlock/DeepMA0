using PACS.Commons.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// FileExplorerView.xaml 的交互逻辑
    /// </summary>
    public partial class FileExplorerView : UserControl
    {
        public String Path { get; set; }

        public FileExplorerView()
        {
            InitializeComponent();
            
            
        }

    }
}
