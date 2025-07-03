using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class AdaptiveConstants
    {
        public const Int32 KNOWN_TEXT_STYLES_COUNT = 9;
        public const Double DefaultExternalMargin = 8.0;
        public const Double SmallExternalMargin = 4.0;

        public const Double DefaultGroupTopMargin = 0.0; // Margin between group items (when not text)
        public const Double DefaultImageMargin = 8.0;

        public const Int32 DefaultColumnWeight = 50;
        public const Int32 SpacingColumnWidth = 8;
        public const Double DefaultOverlayOpacity = 0.2; // 20%
    }
}
