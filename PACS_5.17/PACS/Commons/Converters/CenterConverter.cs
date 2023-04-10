using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PACS.Commons.Converters
{
    /// <summary>
    /// 用于设置控件的渲染中心
    /// </summary>
    class CenterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            double d = (double)value;
            return d / 2;
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            double d = (double)value;
            return d * 2;
        }
    }
}
