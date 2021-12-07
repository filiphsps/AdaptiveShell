using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Manifest;
using NotificationsVisualizerLibrary.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using NotificationsVisualizerLibrary.Model.Enums;
using Windows.UI.Xaml.Media.Imaging;
using NotificationsVisualizerLibrary.Controls;
using System.Diagnostics;
using Windows.Storage;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Helpers;
using Windows.UI.Notifications;
using System.Collections;
using NotificationsVisualizerLibrary.Renderers;
using Windows.UI.Xaml.Shapes;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    internal sealed partial class PreviewTileNotificationRaw : UserControl
    {
        private static int BRANDING_HEIGHT = 28;

        public PreviewTileNotificationRaw()
        {
            this.InitializeComponent();
        }

        private Model.AdaptiveBinding _binding;
        private TileSize _tileSize;

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

            _binding = binding;
            _tileSize = tileSize;

            // Set the background color
            TileContentContainer.Background = new SolidColorBrush(visualElements.BackgroundColor);
            
            UsingPeek = false;


            if (binding.Container != null)
            {
                var container = binding.Container;

                // Calculate the tile margin
                Thickness margin = new Thickness(GetExternalMargin());

                if (isBrandingVisible)
                {
                    switch (tileSize)
                    {
                        case TileSize.Small:
                            margin.Bottom = BRANDING_HEIGHT - 4;
                            break;

                        default:
                            margin.Bottom = BRANDING_HEIGHT;
                            break;
                    }
                }

                // Render the adaptive
                TileContent.Child = AdaptiveRenderer.Render(container, margin);


                // Background image
                var allImages = container.GetAllDescendants().OfType<AdaptiveImage>();
                var backgroundImage = allImages.FirstOrDefault(i => i.Placement == Placement.Background);
                var peekImage = allImages.FirstOrDefault(i => i.Placement == Placement.Peek);
                
                // If we don't support both peek and background, then just peek is used
                if (!binding.SupportedFeatures.BackgroundAndPeekImage && backgroundImage != null && peekImage != null)
                    backgroundImage = null;


                // Calculate overlays
                double backgroundOverlay = 0;

                if (backgroundImage != null)
                {
                    if (backgroundImage.HintOverlay != null)
                        backgroundOverlay = backgroundImage.HintOverlay.Value;
                    else if (binding.HintOverlay != null)
                        backgroundOverlay = binding.HintOverlay.Value;

                    else
                    {
                        // If there's text on the tile, defaults to 20
                        if (container.GetAllDescendants().OfType<AdaptiveTextField>().Any())
                            backgroundOverlay = 20;

                        // Else defaults to no overlay
                        else
                            backgroundOverlay = 0;
                    }

                    // If we're ignoring the dev specified background when there's no text on the tile
                    if (!binding.SupportedFeatures.RespectDevSpecifiedBackgroundOverlayEvenWhenNoTextOnTile)
                    {
                        if (!container.GetAllDescendants().OfType<AdaptiveTextField>().Any())
                            backgroundOverlay = 0;
                    }
                }

                double peekOverlay = 0;

                if (peekImage != null)
                {
                    if (peekImage.HintOverlay != null)
                        peekOverlay = peekImage.HintOverlay.Value;

                    // New in TH2: Binding overlay applies to both peek and background
                    else if (binding.HintOverlay != null && binding.SupportedFeatures.OverlayForBothBackgroundAndPeek)
                        peekOverlay = binding.HintOverlay.Value;

                    // Defaults to 0
                    else
                        peekOverlay = 0;
                }


                if (backgroundImage != null)
                {
                    switch (backgroundImage.HintCrop)
                    {
                        case HintCrop.Circle:
                            BackgroundImageContainer.Child = new CircleImage()
                            {
                                Source = ImageHelper.GetBitmap(backgroundImage.Src),
                                Margin = tileSize == TileSize.Small ? new Thickness(4) : new Thickness(8),
                                OverlayOpacity = backgroundOverlay / 100.0
                            };
                            break;

                        default:
                            BackgroundImageContainer.Child = new Image()
                            {
                                Source = ImageHelper.GetBitmap(backgroundImage.Src),
                                Stretch = Stretch.UniformToFill
                            };
                            BackgroundImageOverlay.Opacity = backgroundOverlay / 100.0;
                            BackgroundImageOverlay.Visibility = Visibility.Visible;
                            break;
                    }
                }

                if (peekImage != null)
                {
                    UsingPeek = true;

                    switch (peekImage.HintCrop)
                    {
                        case HintCrop.Circle:
                            PeekImageContainer.Child = new CircleImage()
                            {
                                Source = ImageHelper.GetBitmap(peekImage.Src),
                                Margin = tileSize == TileSize.Small ? new Thickness(4) : new Thickness(8),
                                OverlayOpacity = peekOverlay / 100.0
                            };
                            break;

                        default:
                            PeekImageContainer.Child = new Grid()
                            {
                                Children =
                                {
                                    new Image()
                                    {
                                        Source = ImageHelper.GetBitmap(peekImage.Src),
                                        Stretch = Stretch.UniformToFill
                                    },
                                    
                                    // Overlay
                                    new Rectangle()
                                    {
                                        Fill = new SolidColorBrush(Colors.Black),
                                        Opacity = peekOverlay / 100.0
                                    }
                                }
                            };
                            break;
                    }

                    PeekRow.Height = new GridLength(1, GridUnitType.Star);
                }
            }
        }

        public bool UsingPeek { get; private set; }

        private double GetExternalMargin()
        {
            switch (_tileSize)
            {
                case TileSize.Small:
                    return AdaptiveConstants.SmallExternalMargin;

                default:
                    return AdaptiveConstants.DefaultExternalMargin;
            }
        }
    }
}
