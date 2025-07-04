using Microsoft.UI.Xaml.Data;
using System;

namespace NotificationsVisualizerLibrary.Converters
{
    internal sealed partial class StringToVisibilityConverter : IValueConverter
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
