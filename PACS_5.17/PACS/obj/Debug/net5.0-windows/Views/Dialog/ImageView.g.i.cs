﻿#pragma checksum "..\..\..\..\..\Views\Dialog\ImageView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "29F5E95352E0D07F90EE7CEC29DF4A89880FEEB6"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf.Transitions;
using PACS.Commons.Converters;
using PACS.ViewModels;
using PACS.Views;
using Prism.DryIoc;
using Prism.Interactivity;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Prism.Services.Dialogs;
using Prism.Unity;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PACS.Views {
    
    
    /// <summary>
    /// ImageView
    /// </summary>
    public partial class ImageView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Grid;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid Viewer;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ClipGrid;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.InkCanvas dcm;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.TranslateTransform dcm_location;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.ScaleTransform dcm_scale;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PACS;V1.0.0.0;component/views/dialog/imageview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Grid = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.Viewer = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.ClipGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.dcm = ((System.Windows.Controls.InkCanvas)(target));
            
            #line 26 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
            this.dcm.MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.dcm_MouseWheel);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
            this.dcm.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.dcm_MouseDown);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\..\..\Views\Dialog\ImageView.xaml"
            this.dcm.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.dcm_PreviewMouseMove);
            
            #line default
            #line hidden
            return;
            case 5:
            this.dcm_location = ((System.Windows.Media.TranslateTransform)(target));
            return;
            case 6:
            this.dcm_scale = ((System.Windows.Media.ScaleTransform)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

