using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace NotificationsVisualizerLibrary
{
    public sealed class TileDensity
    {
        public static TileDensity Desktop()
        {
            return new TileDensity(
                small: new Size(48, 48),
                medium: new Size(100, 100),
                wide: new Size(204, 100),
                large: new Size(204, 204)
                );
        }

        public static TileDensity Tablet()
        {
            return new TileDensity(
                small: new Size(60, 60),
                medium: new Size(125, 125),
                wide: new Size(255, 125),
                large: new Size(255, 255)
                );
        }

        public static TileDensity Mobile(double customDensity)
        {
            return new TileDensity(
                small: new Size(48 * customDensity, 48 * customDensity),
                medium: new Size(100 * customDensity, 100 * customDensity),
                wide: new Size(204 * customDensity, 100 * customDensity),
                large: new Size(204 * customDensity, 204 * customDensity)
                );
        }

        public Size Small { get; private set; }

        public Size Medium { get; private set; }

        public Size Wide { get; private set; }

        public Size Large { get; private set; }

        private TileDensity(Size small, Size medium, Size wide, Size large)
        {
            Small = small;
            Medium = medium;
            Wide = wide;
            Large = large;
        }
    }
}
