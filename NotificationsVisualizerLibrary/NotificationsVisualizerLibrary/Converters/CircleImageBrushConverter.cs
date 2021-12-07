using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class CircleImageBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ImageSource source = value as ImageSource;
            if (source != null)
                return new ImageBrush()
                {
                    ImageSource = source,
                    Stretch = Stretch.UniformToFill
                };

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
