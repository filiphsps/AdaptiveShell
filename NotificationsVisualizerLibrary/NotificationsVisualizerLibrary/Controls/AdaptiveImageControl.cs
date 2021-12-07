using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace NotificationsVisualizerLibrary.Controls
{
    public sealed class AdaptiveImageControl : Control, IAdaptiveControl
    {
        public bool DoesAllContentFit { get; private set; }

        public AdaptiveImageControl()
        {
            this.DefaultStyleKey = typeof(AdaptiveImageControl);
        }

        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(BitmapImage), typeof(AdaptiveImageControl), new PropertyMetadata(default(BitmapImage), OnSourcePropertyChanged));

        public BitmapImage Source
        {
            get { return GetValue(SourceProperty) as BitmapImage; }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as AdaptiveImageControl).OnSourcePropertyChanged(e);
        }

        private void OnSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            BitmapImage oldBitmap = e.OldValue as BitmapImage;

            if (oldBitmap != null)
                oldBitmap.ImageOpened -= Bitmap_ImageOpened;
            
            BitmapImage newBitmap = e.NewValue as BitmapImage;

            if (newBitmap != null)
            {
                newBitmap.ImageOpened += Bitmap_ImageOpened;
            }

            base.InvalidateMeasure();
        }

        private void Bitmap_ImageOpened(object sender, RoutedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            DoesAllContentFit = true;

            if (Source == null || Source.PixelHeight == 0 || Source.PixelWidth == 0)
                return new Size();

            switch (Stretch)
            {
                case AdaptiveImageStretch.None:

                    // If it can't fit, flag it as not fitting
                    if (Source.PixelHeight > availableSize.Height)
                        DoesAllContentFit = false;

                    // We still display it at its full size despite not fitting
                    return new Size(Source.PixelWidth, Source.PixelHeight);




                case AdaptiveImageStretch.UniformToWidth:

                    // If infinite width (this shouldn't ever get hit for tiles, but might as well implement it)
                    if (double.IsInfinity(availableSize.Width))
                    {
                        // If can't fit at its native height
                        if (Source.PixelHeight > availableSize.Height)
                        {
                            DoesAllContentFit = false;
                            return FitToHeight(availableSize.Height); // Then fit it to the height
                        }

                        // Otherwise display at its full size
                        return new Size(Source.PixelWidth, Source.PixelHeight);
                    }


                    if (availableSize.Width <= 0)
                        return new Size();


                    Size fitted = FitToWidth(availableSize.Width);

                    // If all can't fit, flag it as so and fit to its height
                    if (fitted.Height > availableSize.Height)
                    {
                        DoesAllContentFit = false;
                        return FitToHeight(availableSize.Height);
                    }

                    return fitted;


                case AdaptiveImageStretch.Uniform:

                    if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
                        return new Size(Source.PixelWidth, Source.PixelHeight);

                    if (availableSize.Width <= 0 || availableSize.Height <= 0)
                        return new Size();

                    // If need to fit to height
                    if (double.IsInfinity(availableSize.Width) || (!double.IsInfinity(availableSize.Height) && (Source.PixelHeight / (double)Source.PixelWidth) >= (availableSize.Height / availableSize.Width)))
                        return FitToHeight(availableSize.Height);

                    // Otherwise fit to width
                    else
                        return FitToWidth(availableSize.Width);


                default:
                    throw new NotImplementedException();
            }
        }

        private Size FitToHeight(double height)
        {
            if (Source == null || Source.PixelHeight == 0 || Source.PixelWidth == 0)
                return new Size();

            return new Size(Math.Ceiling(height * (Source.PixelWidth / (double)Source.PixelHeight)), height);
        }

        private Size FitToWidth(double width)
        {
            if (Source == null || Source.PixelHeight == 0 || Source.PixelWidth == 0)
                return new Size();

            return new Size(width, Math.Ceiling(width * (Source.PixelHeight / (double)Source.PixelWidth)));
        }

        private Size CalculateDesiredSize(Size availableSize)
        {
            if (Source == null || Source.PixelHeight == 0 || Source.PixelWidth == 0)
                return new Size();

            switch (Stretch)
            {
                case AdaptiveImageStretch.None:
                    return new Size(Source.PixelWidth, Source.PixelHeight);

                case AdaptiveImageStretch.UniformToWidth:

                    if (double.IsInfinity(availableSize.Width))
                        return new Size(Source.PixelWidth, Source.PixelHeight);

                    if (availableSize.Width <= 0)
                        return new Size();

                    return new Size(availableSize.Width, Math.Ceiling(availableSize.Width * (Source.PixelHeight / (double)Source.PixelWidth)));


                case AdaptiveImageStretch.Uniform:

                    if (double.IsInfinity(availableSize.Width) && double.IsInfinity(availableSize.Height))
                        return new Size(Source.PixelWidth, Source.PixelHeight);

                    if (availableSize.Width <= 0 || availableSize.Height <= 0)
                        return new Size();

                    // If need to fit to height
                    if (double.IsInfinity(availableSize.Width) || (!double.IsInfinity(availableSize.Height) && (Source.PixelHeight / (double)Source.PixelWidth) >= (availableSize.Height / availableSize.Width)))
                        return new Size(Math.Ceiling(availableSize.Height * (Source.PixelWidth / (double)Source.PixelHeight)), availableSize.Height);

                    // Otherwise fit to width
                    else
                        return new Size(availableSize.Width, Math.Ceiling(availableSize.Width * (Source.PixelHeight / (double)Source.PixelWidth)));


                default:
                    throw new NotImplementedException();
            }
        }

        private static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(AdaptiveImageStretch), typeof(AdaptiveImageControl), new PropertyMetadata(AdaptiveImageStretch.Uniform, OnStretchPropertyChanged));

        private static void OnStretchPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as AdaptiveImageControl).OnStretchPropertyChanged(e);
        }

        private void OnStretchPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        public AdaptiveImageStretch Stretch
        {
            get { return (AdaptiveImageStretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
    }

    public enum AdaptiveImageStretch
    {
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
