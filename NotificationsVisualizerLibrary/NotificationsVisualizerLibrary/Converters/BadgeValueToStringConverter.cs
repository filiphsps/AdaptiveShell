using System;
using Microsoft.UI.Xaml.Data;
using NotificationsVisualizerLibrary.Model;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class BadgeValueToStringConverter : IValueConverter
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
