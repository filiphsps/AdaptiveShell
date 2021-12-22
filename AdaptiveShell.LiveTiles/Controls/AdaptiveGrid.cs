using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.Controls {
    public sealed class AdaptiveGrid : Grid, IAdaptiveControl {
        public Boolean DoesAllContentFit { get; private set; }

        protected override Size MeasureOverride(Size availableSize) {
            this.DoesAllContentFit = true;

            Size answer = base.MeasureOverride(availableSize);

            if (this.Children != null && this.Children.OfType<IAdaptiveControl>().Any(i => !i.DoesAllContentFit))
                this.DoesAllContentFit = false;

            return answer;
        }
    }
}
