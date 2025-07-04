using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using NotificationsVisualizerLibrary.Model;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class BadgeValueToVisibilityConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var badgeValue = value as BadgeValue;
            if (badgeValue == null)
                return Visibility.Collapsed;

            if (badgeValue.HasBadge())
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
