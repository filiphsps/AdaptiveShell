using System;
using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Model;
using Microsoft.UI.Xaml.Controls;

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
        public void InitializeFromXml(TileSize tileSize, PreviewTileVisualElements visualElements, Boolean isBrandingVisible, AdaptiveBinding binding)
        {
            ArgumentNullException.ThrowIfNull(binding);


            var raw = new PreviewTileNotificationRaw();
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
