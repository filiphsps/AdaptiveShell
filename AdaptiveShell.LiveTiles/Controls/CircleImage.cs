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
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.Controls {
    public sealed class CircleImage : Control, IAdaptiveControl {
        public Boolean DoesAllContentFit { get; private set; }

        public CircleImage() {
            this.DefaultStyleKey = typeof(CircleImage);
        }

        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(BitmapImage), typeof(CircleImage),
            new PropertyMetadata(default(BitmapImage), OnSourcePropertyChanged));

        public BitmapImage Source {
            get => this.GetValue(SourceProperty) as BitmapImage;
            set => this.SetValue(SourceProperty, value);
        }

        private static readonly DependencyProperty OverlayOpacityProperty =
            DependencyProperty.Register("OverlayOpacity", typeof(Double), typeof(CircleImage), new PropertyMetadata(0));

        public Double OverlayOpacity {
            get => (Double)this.GetValue(OverlayOpacityProperty);
            set => this.SetValue(OverlayOpacityProperty, value);
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as CircleImage).OnSourcePropertyChanged(e);
        }

        private void OnSourcePropertyChanged(DependencyPropertyChangedEventArgs e) {
            var oldBitmap = e.OldValue as BitmapImage;

            if (oldBitmap != null)
                oldBitmap.ImageOpened -= this.Bitmap_ImageOpened;

            var newBitmap = e.NewValue as BitmapImage;

            if (newBitmap != null) {
                newBitmap.ImageOpened += this.Bitmap_ImageOpened;
            }

            this.InvalidateMeasure();
        }

        private void Bitmap_ImageOpened(Object sender, RoutedEventArgs e) {
            this.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize) {
            Size desiredSize = this.CalculateDesiredSize(availableSize);

            this.DoesAllContentFit = desiredSize.Height <= availableSize.Height;

            base.MeasureOverride(desiredSize);

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            return base.ArrangeOverride(this.CalculateDesiredSize(finalSize));
        }

        private Size CalculateDesiredSize(Size availableSize) {
            Double min = 0;

            if (this.Source != null)
                min = Math.Min(this.Source.PixelWidth, this.Source.PixelHeight);

            switch (this.Stretch) {
                case CircleImageStretch.UniformToWidthOrHeight:

                    // If width is infinite
                    if (Double.IsInfinity(availableSize.Width)) {
                        // If both dimmensions are infinity
                        if (Double.IsInfinity(availableSize.Height))
                            return new Size(min, min);

                        // Otherwise fit to height
                        else
                            return new Size(availableSize.Height, availableSize.Height);
                    }

                    // Else if height is infinite
                    else if (Double.IsInfinity(availableSize.Height)) {
                        // Fit to width
                        return new Size(availableSize.Width, availableSize.Width);
                    }

                    // Otherwise neither are infinite
                    else {
                        if (availableSize.Width < availableSize.Height)
                            return new Size(availableSize.Width, availableSize.Width);

                        return new Size(availableSize.Height, availableSize.Height);
                    }


                case CircleImageStretch.UniformToWidth:

                    if (Double.IsInfinity(availableSize.Width))
                        return new Size(min, min);

                    return new Size(availableSize.Width, availableSize.Width);


                case CircleImageStretch.None:
                    return new Size(min, min);


                default:
                    throw new NotImplementedException();
            }
        }

        private static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch",
            typeof(CircleImageStretch), typeof(CircleImage),
            new PropertyMetadata(CircleImageStretch.UniformToWidthOrHeight, OnStretchPropertyChanged));

        private static void OnStretchPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as CircleImage).OnStretchPropertyChanged(e);
        }

        private void OnStretchPropertyChanged(DependencyPropertyChangedEventArgs e) {
            this.InvalidateMeasure();
        }

        public CircleImageStretch Stretch {
            get => (CircleImageStretch)this.GetValue(StretchProperty);
            set => this.SetValue(StretchProperty, value);
        }
    }

    public enum CircleImageStretch {
        /// <summary>
        /// The image is resized to fit either the width or height of the control (whichever is smaller)
        /// </summary>
        UniformToWidthOrHeight,

        /// <summary>
        /// The image is resized to fit the width of the control.
        /// </summary>
        UniformToWidth,

        /// <summary>
        /// The image is displayed at its actual size
        /// </summary>
        None
    }
}
