using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace AdaptiveShell.LiveTiles.Converters {
    public sealed class ColorToBrushConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            if (value is Color)
                return new SolidColorBrush((Color)value);

            return value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language) {
            throw new NotImplementedException();
        }
    }
}
