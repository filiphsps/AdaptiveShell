using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NotificationsVisualizerLibrary.Parsers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    internal sealed partial class PreviewTileNotification : UserControl
    {
        public PreviewTileNotification()
        {
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
        public void InitializeFromXml(TileSize tileSize, PreviewTileVisualElements visualElements, bool isBrandingVisible, AdaptiveBinding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");


            PreviewTileNotificationRaw raw = new PreviewTileNotificationRaw();
            raw.InitializeFromXml(tileSize, visualElements, isBrandingVisible, binding);

            if (raw.UsingPeek)
                base.Content = new PeekDisplayerControl()
                {
                    PreviewTileNotificationRaw = raw,
                    PeekStartsOn = PeekContentDisplayed.Content
                };

            else
                base.Content = raw;
        }
    }
}
