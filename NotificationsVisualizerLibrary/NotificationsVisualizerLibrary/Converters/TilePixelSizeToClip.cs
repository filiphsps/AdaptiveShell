using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.Foundation;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class TilePixelSizeToClip : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            if (value is Size)
            {
                var s = (Size)value;

                var clipSize = new Size(s.Width * 3, s.Height);

                return new RectangleGeometry()
                {
                    Rect = new Rect(new Point(s.Width * -1, 0), clipSize)
                };
            }

            return value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
