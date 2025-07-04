using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.Foundation;

namespace NotificationsVisualizerLibrary.Controls
{
    public sealed partial class AdaptiveGrid : Grid, IAdaptiveControl
    {
        public Boolean DoesAllContentFit { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.DoesAllContentFit = true;

            Size answer = base.MeasureOverride(availableSize);

            if (this.Children != null && this.Children.OfType<IAdaptiveControl>().Any(i => !i.DoesAllContentFit))
                this.DoesAllContentFit = false;

            return answer;
        }
    }
}
