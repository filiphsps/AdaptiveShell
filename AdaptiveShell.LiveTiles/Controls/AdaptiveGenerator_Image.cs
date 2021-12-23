using AdaptiveShell.LiveTiles.Models.BaseElements;
using AdaptiveShell.LiveTiles.Models.Enums;
using AdaptiveShell.LiveTiles.Renderers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.Controls {
    internal static class AdaptiveGenerator_Image {
        public static FrameworkElement GenerateImage(
            AdaptiveImage imageNode,
            Double topMargin,
            Boolean isInsideGroup,
            out Boolean needsMargin) {
            needsMargin = !imageNode.HintRemoveMargin.GetValueOrDefault(false);

            FrameworkElement imageControl;
            BitmapImage bmp = ImageHelper.GetBitmap(imageNode.Src);

            HintImageAlign hintAlign = imageNode.HintAlign;
            if (hintAlign == HintImageAlign.Default) {
                hintAlign = HintImageAlign.Stretch;
            }

            HintCrop crop = imageNode.HintCrop;
            if (crop == HintCrop.Circle) {
                CircleImageStretch stretch;

                if (hintAlign == HintImageAlign.Stretch) {
                    stretch = CircleImageStretch.UniformToWidthOrHeight;
                } else
                    stretch = CircleImageStretch.None;

                imageControl = new CircleImage() {
                    Source = bmp,
                    Stretch = stretch
                };
            } else {
                imageControl = new AdaptiveImageControl() {
                    Source = bmp,
                    Stretch = hintAlign == HintImageAlign.Stretch ? (isInsideGroup ? AdaptiveImageStretch.UniformToWidth : AdaptiveImageStretch.Uniform) : AdaptiveImageStretch.None
                };
            }

            switch (hintAlign) {
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

            if (!imageNode.HintRemoveMargin.GetValueOrDefault(false)) {
                imageControl.Margin = new Thickness(0, topMargin, 0, 0);
            }

            return imageControl;
        }
    }
}
