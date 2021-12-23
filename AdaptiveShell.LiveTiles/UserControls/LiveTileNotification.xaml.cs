using AdaptiveShell.LiveTiles.Controls;
using AdaptiveShell.LiveTiles.Models.BaseElements;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.UserControls {
    public sealed partial class LiveTileNotification : UserControl {
        public LiveTileNotification() {
            this.InitializeComponent();
        }

        /// <summary>
        /// This should only be called once. For new tile notification content, create a new instance of this element.
        /// </summary>
        /// <param name="tileSize"></param>
        /// <param name="tilePixelSize"></param>
        /// <param name="visualElements"></param>
        /// <param name="isBrandingVisible"></param>
        /// <param name="binding"></param>
        public void InitializeFromXml(TileSize tileSize, LiveTileVisualElements visualElements, Boolean isBrandingVisible, AdaptiveBinding binding) {
            if (binding == null)
                throw new ArgumentNullException("binding");


            var raw = new LiveTileNotificationRaw();
            raw.InitializeFromXml(tileSize, visualElements, isBrandingVisible, binding);

            if (raw.UsingPeek)
                base.Content = new PeekDisplayerControl() {
                    LiveTileNotificationRaw = raw,
                    PeekStartsOn = PeekContentDisplayed.Content
                };

            else
                base.Content = raw;
        }
    }
}
