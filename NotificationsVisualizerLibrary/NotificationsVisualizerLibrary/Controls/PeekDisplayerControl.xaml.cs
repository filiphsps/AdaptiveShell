﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Linq;
using Windows.Foundation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary.Controls
{
    internal enum PeekContentDisplayed
    {
        PeekImage,
        Content
    }

    internal sealed partial class PeekDisplayerControl : UserControl
    {
        public PeekDisplayerControl()
        {
            this.InitializeComponent();
        }

        #region PreviewTileNotificationRaw

        private static readonly DependencyProperty PreviewTileNotificationRawProperty = DependencyProperty.Register("PreviewTileNotificationRaw", typeof(PreviewTileNotificationRaw), typeof(PeekDisplayerControl), null);

        internal PreviewTileNotificationRaw PreviewTileNotificationRaw
        {
            get { return this.GetValue(PreviewTileNotificationRawProperty) as PreviewTileNotificationRaw; }
            set { this.SetValue(PreviewTileNotificationRawProperty, value); }
        }

        #endregion

        #region PeekStartsOn

        private static readonly DependencyProperty PeekStartsOnProperty = DependencyProperty.Register("PeekStartsOn", typeof(PeekContentDisplayed), typeof(PeekDisplayerControl), new PropertyMetadata(PeekContentDisplayed.PeekImage, OnPeekStartsOnPropertyChanged));

        private static void OnPeekStartsOnPropertyChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PeekDisplayerControl).OnPeekStartsOnPropertyChanged(e);
        }

        private void OnPeekStartsOnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InitializePeekAnimation();
        }

        public PeekContentDisplayed PeekStartsOn
        {
            get { return (PeekContentDisplayed)this.GetValue(PeekStartsOnProperty); }
            set { this.SetValue(PeekStartsOnProperty, value); }
        }

        #endregion

        private void Canvas_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            Size size = e.NewSize;

            if (size.IsEmpty || Double.IsInfinity(size.Height) || Double.IsInfinity(size.Width))
                return;

            // Set clipping so content outside the canvas isn't displayed
            this.CanvasClip.Rect = new Rect(new Point(), size);

            // Set the size of the content container
            this.NotificationContentContainer.Width = size.Width;
            this.NotificationContentContainer.Height = size.Height * 2; // Height gets doubled to support peek

            // And update the peek animation
            this.InitializePeekAnimation();
        }

        private static DoubleAnimationUsingKeyFrames GeneratePeekAnimation(Double normalHeight, PeekContentDisplayed startingContent, TimeSpan timeBetweenSwitch)
        {
            Double valueWhenPeekVisible = 0;
            Double valueWhenContentVisible = normalHeight * -1;
            Double halfValue = normalHeight * -0.5;

            Double initialValue;
            Double switchedValue;

            var durationOfSwitchAnimation = TimeSpan.FromSeconds(1.5);

            switch (startingContent)
            {
                case PeekContentDisplayed.PeekImage:
                    initialValue = valueWhenPeekVisible;
                    switchedValue = valueWhenContentVisible;
                    break;

                case PeekContentDisplayed.Content:
                    initialValue = valueWhenContentVisible;
                    switchedValue = valueWhenPeekVisible;
                    break;

                default:
                    throw new NotImplementedException();
            }

            EasingFunctionBase easingFunction = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseInOut,
                Exponent = 4
            };

            

            var a = new DoubleAnimationUsingKeyFrames();

            // Initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = initialValue,
                KeyTime = new TimeSpan()
            });

            // Holding initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = initialValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(timeBetweenSwitch)
            });

            // Switched position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = switchedValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(durationOfSwitchAnimation),
                EasingFunction = easingFunction
            });

            // Holding switched position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = switchedValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(timeBetweenSwitch)
            });

            // Back to initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame()
            {
                Value = initialValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(durationOfSwitchAnimation),
                EasingFunction = easingFunction
            });


            return a;
        }

        private Storyboard _storyboard;

        private void InitializePeekAnimation()
        {
            Double normalHeight = this.PresentationCanvas.ActualHeight;

            this._storyboard?.Stop();



            this._storyboard = new Storyboard()
            {
                RepeatBehavior = Microsoft.UI.Xaml.Media.Animation.RepeatBehavior.Forever
            };


            DoubleAnimationUsingKeyFrames a = GeneratePeekAnimation(
                normalHeight,
                this.PeekStartsOn,
                TimeSpan.FromSeconds(4));


            this._storyboard.Children.Add(a);

            Storyboard.SetTarget(a, this.TranslateContent);
            Storyboard.SetTargetProperty(a, "Y");

            this._storyboard.Begin();
        }
    }
}
