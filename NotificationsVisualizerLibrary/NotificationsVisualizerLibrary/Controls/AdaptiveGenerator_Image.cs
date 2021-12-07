using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace NotificationsVisualizerLibrary.Controls
{
    internal static class AdaptiveGenerator_Image
    {
        public static FrameworkElement GenerateImage(
            AdaptiveImage imageNode,
            double topMargin,
            bool isInsideGroup,
            out bool needsMargin)
        {
            needsMargin = !imageNode.HintRemoveMargin.GetValueOrDefault(false);

            FrameworkElement imageControl;
            BitmapImage bmp = ImageHelper.GetBitmap(imageNode.Src);

            var hintAlign = imageNode.HintAlign;
            if (hintAlign == HintImageAlign.Default)
            {
                hintAlign = HintImageAlign.Stretch;
            }

            var crop = imageNode.HintCrop;
            if (crop == HintCrop.Circle)
            {
                CircleImageStretch stretch;

                if (hintAlign == HintImageAlign.Stretch)
                {
                    if (imageNode.SupportedFeatures.CircleImageVerticalStretch)
                        stretch = CircleImageStretch.UniformToWidthOrHeight;
                    else
                        stretch = CircleImageStretch.UniformToWidth;
                }

                else
                    stretch = CircleImageStretch.None;

                imageControl = new CircleImage()
                {
                    Source = bmp,
                    Stretch = stretch
                };
            }

            else
            {
                imageControl = new Controls.AdaptiveImageControl()
                {
                    Source = bmp,
                    Stretch = hintAlign == HintImageAlign.Stretch ? (isInsideGroup ? AdaptiveImageStretch.UniformToWidth : AdaptiveImageStretch.Uniform) : AdaptiveImageStretch.None
                };
            }

            switch (hintAlign)
            {
                case HintImageAlign.Left:
                    imageControl.HorizontalAlignment = HorizontalAlignment.Left;
                    break;

                case HintImageAlign.Center:
                    imageControl.HorizontalAlignment = HorizontalAlignment.Center;
                    break;

                case HintImageAlign.Right:
                    imageControl.HorizontalAlignment = HorizontalAlignment.Right;
                    break;

                default:
                    imageControl.HorizontalAlignment = HorizontalAlignment.Stretch; //images fill available width
                    break;
            }

            imageControl.VerticalAlignment = VerticalAlignment.Top; //images don't fill available height

            if (!imageNode.HintRemoveMargin.GetValueOrDefault(false))
            {
                imageControl.Margin = new Thickness(0, topMargin, 0, 0);
            }

            return imageControl;
        }
    }
}
