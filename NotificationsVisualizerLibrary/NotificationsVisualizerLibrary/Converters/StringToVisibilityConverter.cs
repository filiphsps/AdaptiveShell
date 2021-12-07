using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    internal sealed class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool answer = value is string && !string.IsNullOrWhiteSpace(value as string);

            if (parameter != null)
            {
                return !answer;
            }

            return answer;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
