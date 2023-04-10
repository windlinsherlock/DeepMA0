﻿using MaterialDesignThemes.Wpf;
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
using System.Windows.Shapes;

namespace PACS.Views
{
    /// <summary>
    ///自定义弹窗父窗口
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : Window,IDialogWindow
    {
        public DialogWindow()
        {
            InitializeComponent();
            //因为没有头部栏，写拖动方法
            MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };
        }

        public IDialogResult Result { get; set; }
    }
}
