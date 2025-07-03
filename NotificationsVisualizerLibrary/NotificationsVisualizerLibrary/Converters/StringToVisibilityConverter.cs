using Microsoft.UI.Xaml.Data;
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
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            Boolean answer = value is String && !String.IsNullOrWhiteSpace(value as String);

            if (parameter != null)
            {
                return !answer;
            }

            return answer;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
