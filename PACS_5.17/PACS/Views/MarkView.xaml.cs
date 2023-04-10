using Newtonsoft.Json.Linq;
using PACS.Common;
using PACS.Commons;
using PACS.Commons.Entities;
using PACS.Commons.Events;
using PACS.Commons.Models;
using PACS.Commons.Models.Shapes;
using PACS.Extensions;
using PACS.Services;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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



namespace PACS.Views
{
    /// <summary>
    /// MarkView.xaml 的交互逻辑
    /// </summary>
    public partial class MarkView : UserControl
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogHostService dialog;
        private readonly DataContext dataContext;

        private readonly ICloudService cloudService;
        private readonly IAutoLableService autoLableService;

        private readonly UserConfiguration userConfiguration;
        private readonly IDialogService dialogService;
        private readonly IAutoDiagnoseService autoDiagnoseService;
        public MarkView(IEventAggregator eventAggregator,DataContext dataContext, IDialogHostService dialog,
            ICloudService cloudService,IAutoLableService autoLableService, IDialogService dialogService,
            UserConfiguration userConfiguration,IAutoDiagnoseService autoDiagnoseService)
        {
            InitializeComponent();

            this.dialog = dialog;
            this.eventAggregator = eventAggregator;
            this.dataContext = dataContext;

            this.cloudService = cloudService;
            this.autoLableService = autoLableService;

            this.userConfiguration = userConfiguration;
            this.dialogService = dialogService;
            this.autoDiagnoseService = autoDiagnoseService;
            eventAggregator.GetEvent<MessageEvent>().Subscribe(LoadImage, arg => arg.Filter.Equals("Mark"));
            eventAggregator.GetEvent<MessageEvent>().Subscribe(ColorChanged, arg => arg.Filter.Equals("Color"));
        }


        String FileItemId;


        Point lastPoint; // 上次右键点击的点

        ShapeType EditMode = ShapeType.Any;
        bool isDrawing = false;
        ShapeInfo MyShape;
        System.Windows.Ink.Stroke lastStroke;
        string tag = "病灶区域";
        byte[] byteImage;  //当前图片的字节数组
        string fileName;   //当前图片的文件名
        private void ColorChanged(MessageModel message)
        {
            var color = (Color)message.Object;

            dcm.DefaultDrawingAttributes.Color = color;

            tag = message.Message;
        }

        private async void LoadImage(MessageModel obj)
        {
            Init();
            this.FileItemId = obj.Message;

            var item = dataContext.FileItems.Single(i => i.FileItemId == obj.Message);

            // 若文件不存在，则下载
            if (item.Image == null) 
            {
                var response = await cloudService.GetFileItem(obj.Message);

                if (response.Status)
                {
                    item.Image = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>((string)response.Result);
                    dataContext.SaveChanges();

                    // 提醒更新文件“云端”状态
                    eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
                    {
                        Filter = "Download",
                        Message = item.FileItemId,
                    });
                }
            }


            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new System.IO.MemoryStream(item.Image);
            image.EndInit();


            // 设置画布大小
            dcm.Height = image.Height;
            dcm.Width = image.Width;
            ClipGrid.Width = image.Width;
            ClipGrid.Height = image.Height;

            


            dcm.Background = new ImageBrush(image);

            byteImage = item.Image;
            fileName = item.Name;
        }

        private void Init()
        {
            dcm.Strokes.Clear();
            dcm.IsEnabled = true;
            lastPoint = new Point(0, 0);

            // 重置缩放、平移
            dcm_location.X = 0;
            dcm_location.Y = 0;
            dcm_scale.ScaleX = 1;
            dcm_scale.ScaleY = 1;


            dcm.DefaultDrawingAttributes.Color = Color.FromRgb(255, 0, 0);

            ChooseButton.SelectedIndex = 0;
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

        /// <summary>
        /// 记录鼠标按下操作，用于绘图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dcm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(dcm);


            
            // 重写非曲线绘图
            if (EditMode != ShapeType.Any && e.ChangedButton == MouseButton.Left)
            {
                if (!isDrawing)
                {
                    MyShape = ShapeFactory.GetShapeInfo(EditMode);
                    isDrawing = true;
                }
                
                MyShape.AddPoint(lastPoint);



                if (MyShape.isFinished)
                {
                    dcm.Strokes.Last().AddPropertyData(new Guid("12345678-9012-3456-7890-123456789000"), EditMode.ToString());
                    dcm.Strokes.Last().AddPropertyData(new Guid("12345678-9012-3456-7890-123456789001"), tag);

                    // 裁剪图片
                    if (EditMode == ShapeType.Square)
                    {

                        int x = (int)MyShape.Stroke.StylusPoints[0].X;
                        int y = (int)MyShape.Stroke.StylusPoints[0].Y;
                        int dx = (int)(MyShape.Stroke.StylusPoints[2].X - x);
                        int dy = (int)(MyShape.Stroke.StylusPoints[2].Y - y);

                        // 获取图像
                        ImageBrush brush = (ImageBrush)dcm.Background;
                        BitmapSource source = (BitmapSource)brush.ImageSource;

                        // 裁剪
                        CroppedBitmap image = new CroppedBitmap(source, new Int32Rect(x, y, dx, dy));

                        // 重置画布大小
                        dcm.Height = image.PixelHeight;
                        dcm.Width = image.PixelWidth;
                        ClipGrid.Width = image.PixelWidth;
                        ClipGrid.Height = image.PixelHeight;



                        dcm.Background = new ImageBrush(image);

                        dcm_location.X = 0;
                        dcm_location.Y = 0;
                        dcm.Strokes.Clear();

                    }



                    EditMode = ShapeType.Any;
                    isDrawing = false;
                    lastStroke = null;
                    //dcm.EditingMode = InkCanvasEditingMode.Ink;


                }
                    
                


            }
        }

        /// <summary>
        /// 截取鼠标移动，用于绘图以及移动图像的判断
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dcm_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                
                MyShape.Draw(e.GetPosition(dcm), dcm.DefaultDrawingAttributes);

                if (lastStroke != null)
                    dcm.Strokes.Remove(lastStroke);
                dcm.Strokes.Add(MyShape.Stroke);
                

                lastStroke = MyShape.Stroke;
            }

            // 移动图像
            if (e.RightButton.Equals(MouseButtonState.Pressed))
            {
                double x = e.GetPosition(dcm).X;
                double y = e.GetPosition(dcm).Y;
                dcm_location.X = dcm_location.X + x - lastPoint.X;
                dcm_location.Y = dcm_location.Y + y - lastPoint.Y;
            }
        }


        #region 工具栏
        private void Select(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Any;
            dcm.EditingMode = InkCanvasEditingMode.Select;
        }

        private void DrawAny(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Any;
            dcm.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void Erase(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Any;
            dcm.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void DrawLine(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Line;
            dcm.EditingMode = InkCanvasEditingMode.None;
        }

        private void DrawRectangle(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Rectangle;
            dcm.EditingMode = InkCanvasEditingMode.None;
        }

        private void DrawSquare(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Square;
            dcm.EditingMode = InkCanvasEditingMode.None;
        }

        private void DrawPolygon(object sender, RoutedEventArgs e)
        {
            EditMode = ShapeType.Polygon;
            dcm.EditingMode = InkCanvasEditingMode.None;
        }
        #endregion

        /// <summary>
        /// 保存分类结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Save(object sender, RoutedEventArgs e)
        {
            if (ChooseButton.SelectedIndex == 0)
            {
                MessageBox.Show("请先进行标注");
                return;
            }

            Shared.Entities.FileMaskModel exsit = null;

            try
            {
                exsit = dataContext.FileMasks.First(i => i.FileItemId.Equals(FileItemId) && i.CreatedBy.Equals(userConfiguration.UserId));
            }
            catch (Exception ex)
            {

            }

            Shared.Entities.FileMaskModel mask = new Shared.Entities.FileMaskModel();

            mask.FileItemId = FileItemId;

            
            if (ChooseButton.SelectedIndex == 1)
                mask.IsPositive = true;
            else
                mask.IsPositive = false;

            List<CustomStroke> content = new List<CustomStroke>();
            foreach(var item in dcm.Strokes)
            {
                string type = "Any", tag = "";
                if (item.ContainsPropertyData(new Guid("12345678-9012-3456-7890-123456789000")))
                {
                    type = item.GetPropertyData(new Guid("12345678-9012-3456-7890-123456789000")).ToString();
                }

                if (item.ContainsPropertyData(new Guid("12345678-9012-3456-7890-123456789001")))
                {
                    tag = item.GetPropertyData(new Guid("12345678-9012-3456-7890-123456789001")).ToString();
                }

                content.Add(new CustomStroke { Type = type, Value = item.StylusPoints,Color = item.DrawingAttributes.Color,Tag = tag });
            }
            mask.Content = Newtonsoft.Json.JsonConvert.SerializeObject(content);

            mask.CreatedBy = userConfiguration.UserId;

            if(exsit != null)
            {
                mask.FileMaskId = exsit.FileMaskId;
                exsit.IsPositive = mask.IsPositive;
                exsit.Content = mask.Content;
            }
            else
            {
                dataContext.FileMasks.Add(mask);
            }

            dataContext.SaveChanges();

            var result = await cloudService.UploadMask(new Shared.DTOs.FileMaskDTO { FileMaskId = mask.FileMaskId, Content = mask.Content, FileItemId = mask.FileItemId, IsPositive = mask.IsPositive });

            if (result != null)
            {
                ;
            }


            /*String directory = "D:\\" + FileItemId;
            System.IO.FileStream fileStream = new System.IO.FileStream(directory + ".isf", System.IO.FileMode.Create);
            dcm.Strokes.Save(fileStream);*/

            

        }

        /// <summary>
        /// 读取分类结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load(object sender, RoutedEventArgs e)
        {
            try
            {
                var mask = dataContext.FileMasks.Single(i => i.FileItemId.Equals(FileItemId) && i.CreatedBy.Equals(userConfiguration.UserId));

                if (mask.IsPositive)
                    ChooseButton.SelectedIndex = 1;
                else
                    ChooseButton.SelectedIndex = 2;


                List<CustomStroke> content = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomStroke>>(mask.Content);
                foreach (CustomStroke stroke in content)
                {
                    // 传递Tag参数以进行创建
                    eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
                    {
                        Filter = "Tag",
                        Object = stroke.Color,
                        Message = stroke.Tag
                    });

                    MyShape = ShapeFactory.GetShapeInfo(stroke.Type);
                    // 读取自定义形状
                    if (MyShape != null)
                        dcm.Strokes.Add(MyShape.Draw(stroke.Tag, stroke.Value, stroke.Color));
                    else
                    {
                        System.Windows.Ink.Stroke any = new System.Windows.Ink.Stroke(stroke.Value);
                        any.DrawingAttributes.Color = stroke.Color;
                        any.AddPropertyData(new Guid("12345678-9012-3456-7890-123456789001"),stroke.Tag);
                        dcm.Strokes.Add(any);
                    }
                }

                /*String directory = "D:\\" + FileItemId;

                System.IO.FileStream fileStream = new System.IO.FileStream(directory + ".isf", System.IO.FileMode.Open);

                dcm.Strokes = new System.Windows.Ink.StrokeCollection(fileStream);*/
            }
            catch
            {
                eventAggregator.SendMessage("当前文件未进行过标注");
            }
        }


        /// <summary>
        /// 调用云端模型进行分类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AutoDiagnose(object sender, RoutedEventArgs e)
        {
            // 设置等待特效
            //Classify.Visibility = Visibility.Hidden;
            //ProgressBar.Visibility = Visibility.Visible;


            //// 等待云端结果
            //var response = await autoLableService.Classify(FileItemId);


            //// 根据云端结果进行标注
            //if (response.Status)
            //{
            //    if ((bool)response.Result)
            //    {
            //        eventAggregator.SendMessage("分类结果为：骨折");
            //        ChooseButton.SelectedIndex = 1;
            //    }

            //    else
            //    {
            //        eventAggregator.SendMessage("分类结果为：未骨折");
            //        ChooseButton.SelectedIndex = 2;
            //    }

            //}
            //else
            //{
            //    //eventAggregator.SendMessage(response.Message);
            //}


            //Classify.Visibility = Visibility.Visible;
            //ProgressBar.Visibility = Visibility.Hidden;

            //DialogParameters param = new DialogParameters();
            //var dialogResult = await dialog.ShowDialog("ImageView", param);

            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "Navigate",
                Message = "2",
            });
            eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel()
            {
                Filter = "AutoDiagnoseImage",
                Message = FileItemId,
            });
        }

        private async void AutoMarkClick(object sender, RoutedEventArgs e)
        {
            eventAggregator.UpdateLoading(new UpdateModel { IsOpen = true });
            var response = await autoDiagnoseService.AutoDiagnose(byteImage);
            if (response != null && response.Status)
            {
                var result = (JObject)response.Result;
                string labelImageStr = result["LabelImage"].ToString();
                byte[] labelImageByte = Convert.FromBase64String(labelImageStr);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(labelImageByte);
                bmp.EndInit();
                eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
                DialogParameters keys = new DialogParameters();
                keys.Add("autoMarkImage", bmp);
                keys.Add("imageName", fileName);
                //后面写"DialogWindow"说明使用自定义父窗口，不写则使用默认窗口
                dialogService.Show("AutoMarkImageView", keys, callback => { }, "DialogWindow");
            }
            else
            {
                eventAggregator.UpdateLoading(new UpdateModel { IsOpen = false });
                eventAggregator.SendMessage("自动标记失败");
            }

        }



        /// <summary>
        /// 侧边栏滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            ScrollViewer.RaiseEvent(eventArg);
        }

        /// <summary>
        /// 为曲线增加Tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dcm_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            e.Stroke.AddPropertyData(new Guid("12345678-9012-3456-7890-123456789001"), tag);
        }
    }



    public class CustomStroke
    {
        public string Type { get; set; }
        public StylusPointCollection Value { get; set; }

        public Color Color { get; set; }

        public string Tag { get; set; }
    }
}
