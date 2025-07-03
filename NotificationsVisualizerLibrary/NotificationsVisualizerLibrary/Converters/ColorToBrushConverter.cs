using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class ColorToBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            if (value is Color)
                return new SolidColorBrush((Color)value);

            return value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
