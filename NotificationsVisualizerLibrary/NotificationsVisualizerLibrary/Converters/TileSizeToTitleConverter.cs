using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NotificationsVisualizerLibrary.Converters
{
    public sealed class TileSizeToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string tileSize = value as string;

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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
