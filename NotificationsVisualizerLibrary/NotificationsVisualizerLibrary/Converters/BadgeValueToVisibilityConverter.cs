using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class BadgeValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BadgeValue badgeValue = value as BadgeValue;
            if (badgeValue == null)
                return Visibility.Collapsed;

            if (badgeValue.HasBadge())
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
