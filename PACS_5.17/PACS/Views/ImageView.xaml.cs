using MaterialDesignThemes.Wpf;
using PACS.Common;
using PACS.Commons.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Services.Dialogs;
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
    /// ImageView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageView : UserControl
    {
        Point lastPoint; // 上次右键点击的点
        

        public ImageView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            dcm.Strokes.Clear();
            dcm.IsEnabled = true;
            lastPoint = new Point(0, 0);

            // 重置缩放、平移
            dcm_location.X = 0;
            dcm_location.Y = 0;
            dcm_scale.ScaleX = 1;
            dcm_scale.ScaleY = 1;

            dcm.EditingMode = InkCanvasEditingMode.None;

            LoadImage();
        }

        

        private async void LoadImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            //image.UriSource = new Uri("C:\\Users\\Hollow51123\\Desktop\\heatmap\\dist\\outcome\\imposed.png");
            image.EndInit();


            // 设置画布大小
            dcm.Height = image.Height;
            dcm.Width = image.Width;
            ClipGrid.Width = image.Width;
            ClipGrid.Height = image.Height;




            dcm.Background = new ImageBrush(image);
        }

        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dcm_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta > 0)
            {
                if (dcm_scale.ScaleX > 10)
                    return;
                dcm_scale.ScaleX *= 1.1;
                dcm_scale.ScaleY *= 1.1;
            }
            else
            {
                if (dcm_scale.ScaleX < 0.2)
                    return;
                dcm_scale.ScaleX *= 0.909;
                dcm_scale.ScaleY *= 0.909;

            }
        }

        private void dcm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(dcm);
        }

        /// <summary>
        /// 截取鼠标移动，用于绘图以及移动图像的判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dcm_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // 移动图像
            if (e.RightButton.Equals(MouseButtonState.Pressed))
            {
                double x = e.GetPosition(dcm).X;
                double y = e.GetPosition(dcm).Y;
                dcm_location.X = dcm_location.X + x - lastPoint.X;
                dcm_location.Y = dcm_location.Y + y - lastPoint.Y;
            }
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
