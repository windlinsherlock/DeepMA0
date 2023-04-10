using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PACS.Commons.Converters
{
    class NagateConverter : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return System.Windows.DependencyProperty.UnsetValue;
            bool d = (bool)value;
            if (d)
                return System.Windows.Visibility.Hidden; 
            return System.Windows.Visibility.Visible;
        }
        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return System.Windows.DependencyProperty.UnsetValue;

            bool d = (bool)value;
            return !d;
        }
    }
}
