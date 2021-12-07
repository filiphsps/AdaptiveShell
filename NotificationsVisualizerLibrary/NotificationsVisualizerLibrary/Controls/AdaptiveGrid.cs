using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationsVisualizerLibrary.Controls
{
    public sealed class AdaptiveGrid : Grid, IAdaptiveControl
    {
        public bool DoesAllContentFit { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            DoesAllContentFit = true;

            Size answer = base.MeasureOverride(availableSize);

            if (Children != null && Children.OfType<IAdaptiveControl>().Any(i => !i.DoesAllContentFit))
                DoesAllContentFit = false;

            return answer;
        }
    }
}
