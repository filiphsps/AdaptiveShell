using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Model;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class BadgeValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is BadgeValue)
                return (value as BadgeValue).ToString();

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
                return BadgeValue.Parse(value as string);

            return BadgeValue.Parse(null);
        }
    }
}
