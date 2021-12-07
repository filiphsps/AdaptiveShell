using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class AdaptiveConstants
    {
        public const int KNOWN_TEXT_STYLES_COUNT = 9;
        public const double DefaultExternalMargin = 8.0;
        public const double SmallExternalMargin = 4.0;

        public const double DefaultGroupTopMargin = 0.0; // Margin between group items (when not text)
        public const double DefaultImageMargin = 8.0;

        public const int DefaultColumnWeight = 50;
        public const int SpacingColumnWidth = 8;
        public const double DefaultOverlayOpacity = 0.2; // 20%
    }
}
