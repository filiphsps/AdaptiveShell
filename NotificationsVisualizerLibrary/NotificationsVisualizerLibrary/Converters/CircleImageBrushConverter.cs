using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed partial class CircleImageBrushConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            var source = value as ImageSource;
            if (source != null)
                return new ImageBrush()
                {
                    ImageSource = source,
                    Stretch = Stretch.UniformToFill
                };

            return value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
