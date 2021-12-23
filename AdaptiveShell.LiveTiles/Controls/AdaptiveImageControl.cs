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
    public sealed class AdaptiveImageControl : Control, IAdaptiveControl {
        public Boolean DoesAllContentFit { get; private set; }

        public AdaptiveImageControl() {
            this.DefaultStyleKey = typeof(AdaptiveImageControl);
        }

        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(BitmapImage), typeof(AdaptiveImageControl),
            new PropertyMetadata(default(BitmapImage), OnSourcePropertyChanged));

        public BitmapImage Source {
            get => this.GetValue(SourceProperty) as BitmapImage;
            set => this.SetValue(SourceProperty, value);
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as AdaptiveImageControl).OnSourcePropertyChanged(e);
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
            this.DoesAllContentFit = true;

            if (this.Source == null || this.Source.PixelHeight == 0 || this.Source.PixelWidth == 0)
                return new Size();

            switch (this.Stretch) {
                case AdaptiveImageStretch.None:

                    // If it can't fit, flag it as not fitting
                    if (this.Source.PixelHeight > availableSize.Height)
                        this.DoesAllContentFit = false;

                    // We still display it at its full size despite not fitting
                    return new Size(this.Source.PixelWidth, this.Source.PixelHeight);




                case AdaptiveImageStretch.UniformToWidth:

                    // If infinite width (this shouldn't ever get hit for tiles, but might as well implement it)
                    if (Double.IsInfinity(availableSize.Width)) {
                        // If can't fit at its native height
                        if (this.Source.PixelHeight > availableSize.Height) {
                            this.DoesAllContentFit = false;
                            return this.FitToHeight(availableSize.Height); // Then fit it to the height
                        }

                        // Otherwise display at its full size
                        return new Size(this.Source.PixelWidth, this.Source.PixelHeight);
                    }


                    if (availableSize.Width <= 0)
                        return new Size();


                    Size fitted = this.FitToWidth(availableSize.Width);

                    // If all can't fit, flag it as so and fit to its height
                    if (fitted.Height > availableSize.Height) {
                        this.DoesAllContentFit = false;
                        return this.FitToHeight(availableSize.Height);
                    }

                    return fitted;


                case AdaptiveImageStretch.Uniform:

                    if (Double.IsInfinity(availableSize.Width) && Double.IsInfinity(availableSize.Height))
                        return new Size(this.Source.PixelWidth, this.Source.PixelHeight);

                    if (availableSize.Width <= 0 || availableSize.Height <= 0)
                        return new Size();

                    // If need to fit to height
                    if (Double.IsInfinity(availableSize.Width) || (!Double.IsInfinity(availableSize.Height) && (this.Source.PixelHeight / (Double)this.Source.PixelWidth) >= (availableSize.Height / availableSize.Width)))
                        return this.FitToHeight(availableSize.Height);

                    // Otherwise fit to width
                    else
                        return this.FitToWidth(availableSize.Width);


                default:
                    throw new NotImplementedException();
            }
        }

        private Size FitToHeight(Double height) {
            if (this.Source == null || this.Source.PixelHeight == 0 || this.Source.PixelWidth == 0)
                return new Size();

            return new Size(Math.Ceiling(height * (this.Source.PixelWidth / (Double)this.Source.PixelHeight)), height);
        }

        private Size FitToWidth(Double width) {
            if (this.Source == null || this.Source.PixelHeight == 0 || this.Source.PixelWidth == 0)
                return new Size();

            return new Size(width, Math.Ceiling(width * (this.Source.PixelHeight / (Double)this.Source.PixelWidth)));
        }

        private Size CalculateDesiredSize(Size availableSize) {
            if (this.Source == null || this.Source.PixelHeight == 0 || this.Source.PixelWidth == 0)
                return new Size();

            switch (this.Stretch) {
                case AdaptiveImageStretch.None:
                    return new Size(this.Source.PixelWidth, this.Source.PixelHeight);

                case AdaptiveImageStretch.UniformToWidth:

                    if (Double.IsInfinity(availableSize.Width))
                        return new Size(this.Source.PixelWidth, this.Source.PixelHeight);

                    if (availableSize.Width <= 0)
                        return new Size();

                    return new Size(availableSize.Width, Math.Ceiling(availableSize.Width * (this.Source.PixelHeight / (Double)this.Source.PixelWidth)));


                case AdaptiveImageStretch.Uniform:

                    if (Double.IsInfinity(availableSize.Width) && Double.IsInfinity(availableSize.Height))
                        return new Size(this.Source.PixelWidth, this.Source.PixelHeight);

                    if (availableSize.Width <= 0 || availableSize.Height <= 0)
                        return new Size();

                    // If need to fit to height
                    if (Double.IsInfinity(availableSize.Width) || (!Double.IsInfinity(availableSize.Height) && (this.Source.PixelHeight / (Double)this.Source.PixelWidth) >= (availableSize.Height / availableSize.Width)))
                        return new Size(Math.Ceiling(availableSize.Height * (this.Source.PixelWidth / (Double)this.Source.PixelHeight)), availableSize.Height);

                    // Otherwise fit to width
                    else
                        return new Size(availableSize.Width, Math.Ceiling(availableSize.Width * (this.Source.PixelHeight / (Double)this.Source.PixelWidth)));


                default:
                    throw new NotImplementedException();
            }
        }

        private static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(AdaptiveImageStretch), typeof(AdaptiveImageControl), new PropertyMetadata(AdaptiveImageStretch.Uniform, OnStretchPropertyChanged));

        private static void OnStretchPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as AdaptiveImageControl).OnStretchPropertyChanged(e);
        }

        private void OnStretchPropertyChanged(DependencyPropertyChangedEventArgs e) {
            this.InvalidateMeasure();
        }

        public AdaptiveImageStretch Stretch {
            get => (AdaptiveImageStretch)this.GetValue(StretchProperty);
            set => this.SetValue(StretchProperty, value);
        }
    }

    public enum AdaptiveImageStretch {
        /// <summary>
        /// The image is resized to fit in the destination dimensions both vertically and horizontally.
        /// </summary>
        Uniform,

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
