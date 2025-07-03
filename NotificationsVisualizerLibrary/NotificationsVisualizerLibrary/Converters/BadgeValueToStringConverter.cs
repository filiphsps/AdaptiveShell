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
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            if (value is BadgeValue)
                return (value as BadgeValue).ToString();

            return "";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            if (value is String)
                return BadgeValue.Parse(value as String);

            return BadgeValue.Parse(null);
        }
    }
}
