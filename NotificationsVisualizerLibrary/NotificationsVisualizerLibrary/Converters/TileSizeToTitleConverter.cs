using Microsoft.UI.Xaml.Data;
using System;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class TileSizeToTitleConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, String language)
        {
            String tileSize = value as String;

            if (tileSize == null)
                return value;

            switch (tileSize.ToLower())
            {
                case "small":
                    return "TileSmall";

                case "medium":
                    return "TileMedium";

                case "wide":
                    return "TileWide";

                case "large":
                    return "TileLarge (Desktop Only)";

                default:
                    return tileSize;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
