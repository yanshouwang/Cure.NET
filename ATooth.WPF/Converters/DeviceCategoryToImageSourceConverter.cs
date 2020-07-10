using ATooth.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace ATooth.WPF.Converters
{
    class DeviceCategoryToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = string.Empty;
            var category = (DeviceCategory)value;
            switch (category)
            {
                case DeviceCategory.ATC:
                    source = "/Assets/ATC.png";
                    break;
                case DeviceCategory.UTC:
                    source = "/Assets/UTC.png";
                    break;
                case DeviceCategory.APC2:
                    source = "/Assets/APC2.png";
                    break;
                case DeviceCategory.PGC:
                    source = "/Assets/PGC.png";
                    break;
                default:
                    break;
            }
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
