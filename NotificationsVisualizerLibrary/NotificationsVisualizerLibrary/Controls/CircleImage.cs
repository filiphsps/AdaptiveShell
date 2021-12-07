using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NotificationsVisualizerLibrary.Controls
{
    public sealed class CircleImage : Control, IAdaptiveControl
    {
        public bool DoesAllContentFit { get; private set; }

        public CircleImage()
        {
            this.DefaultStyleKey = typeof(CircleImage);
        }

        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(BitmapImage), typeof(CircleImage), new PropertyMetadata(default(BitmapImage), OnSourcePropertyChanged));

        public BitmapImage Source
        {
            get { return GetValue(SourceProperty) as BitmapImage; }
            set { SetValue(SourceProperty, value); }
        }

        private static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register("OverlayOpacity", typeof(double), typeof(CircleImage), new PropertyMetadata(0));

        public double OverlayOpacity
        {
            get { return (double)GetValue(OverlayOpacityProperty); }
            set { SetValue(OverlayOpacityProperty, value); }
        }

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CircleImage).OnSourcePropertyChanged(e);
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
            Size desiredSize = CalculateDesiredSize(availableSize);

            DoesAllContentFit = desiredSize.Height <= availableSize.Height;

            base.MeasureOverride(desiredSize);

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(CalculateDesiredSize(finalSize));
        }

        private Size CalculateDesiredSize(Size availableSize)
        {
            double min = 0;

            if (Source != null)
                min = Math.Min(Source.PixelWidth, Source.PixelHeight);

            switch (Stretch)
            {
                case CircleImageStretch.UniformToWidthOrHeight:

                    // If width is infinite
                    if (double.IsInfinity(availableSize.Width))
                    {
                        // If both dimmensions are infinity
                        if (double.IsInfinity(availableSize.Height))
                            return new Size(min, min);

                        // Otherwise fit to height
                        else
                            return new Size(availableSize.Height, availableSize.Height);
                    }

                    // Else if height is infinite
                    else if (double.IsInfinity(availableSize.Height))
                    {
                        // Fit to width
                        return new Size(availableSize.Width, availableSize.Width);
                    }

                    // Otherwise neither are infinite
                    else
                    {
                        if (availableSize.Width < availableSize.Height)
                            return new Size(availableSize.Width, availableSize.Width);

                        return new Size(availableSize.Height, availableSize.Height);
                    }


                case CircleImageStretch.UniformToWidth:

                    if (double.IsInfinity(availableSize.Width))
                        return new Size(min, min);

                    return new Size(availableSize.Width, availableSize.Width);


                case CircleImageStretch.None:
                    return new Size(min, min);


                default:
                    throw new NotImplementedException();
            }
        }

        private static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(CircleImageStretch), typeof(CircleImage), new PropertyMetadata(CircleImageStretch.UniformToWidthOrHeight, OnStretchPropertyChanged));

        private static void OnStretchPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CircleImage).OnStretchPropertyChanged(e);
        }

        private void OnStretchPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        public CircleImageStretch Stretch
        {
            get { return (CircleImageStretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
    }

    public enum CircleImageStretch
    {
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
