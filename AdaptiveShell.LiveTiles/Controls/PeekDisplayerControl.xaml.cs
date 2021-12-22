using AdaptiveShell.LiveTiles.UserControls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.Controls {
    internal enum PeekContentDisplayed {
        PeekImage,
        Content
    }

    internal sealed partial class PeekDisplayerControl : UserControl {
        public PeekDisplayerControl() {
            this.InitializeComponent();
        }

        #region LiveTileNotificationRaw

        private static readonly DependencyProperty LiveTileNotificationRawProperty =
            DependencyProperty.Register("LiveTileNotificationRaw", typeof(LiveTileNotificationRaw),
                typeof(PeekDisplayerControl), null);

        internal LiveTileNotificationRaw LiveTileNotificationRaw {
            get => this.GetValue(LiveTileNotificationRawProperty) as LiveTileNotificationRaw;
            set => this.SetValue(LiveTileNotificationRawProperty, value);
        }

        #endregion

        #region PeekStartsOn

        private static readonly DependencyProperty PeekStartsOnProperty = DependencyProperty.Register("PeekStartsOn", typeof(PeekContentDisplayed), typeof(PeekDisplayerControl), new PropertyMetadata(PeekContentDisplayed.PeekImage, OnPeekStartsOnPropertyChanged));

        private static void OnPeekStartsOnPropertyChanged(Object sender, DependencyPropertyChangedEventArgs e) {
            (sender as PeekDisplayerControl).OnPeekStartsOnPropertyChanged(e);
        }

        private void OnPeekStartsOnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            this.InitializePeekAnimation();
        }

        public PeekContentDisplayed PeekStartsOn {
            get => (PeekContentDisplayed)this.GetValue(PeekStartsOnProperty);
            set => this.SetValue(PeekStartsOnProperty, value);
        }

        #endregion

        private void Canvas_SizeChanged(Object sender, SizeChangedEventArgs e) {
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

        private static DoubleAnimationUsingKeyFrames GeneratePeekAnimation(Double normalHeight, PeekContentDisplayed startingContent, TimeSpan timeBetweenSwitch) {
            Double valueWhenPeekVisible = 0;
            Double valueWhenContentVisible = normalHeight * -1;
            Double halfValue = normalHeight * -0.5;

            Double initialValue;
            Double switchedValue;

            var durationOfSwitchAnimation = TimeSpan.FromSeconds(1.5);

            switch (startingContent) {
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

            EasingFunctionBase easingFunction = new ExponentialEase() {
                EasingMode = EasingMode.EaseInOut,
                Exponent = 4
            };



            var a = new DoubleAnimationUsingKeyFrames();

            // Initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame() {
                Value = initialValue,
                KeyTime = new TimeSpan()
            });

            // Holding initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame() {
                Value = initialValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(timeBetweenSwitch)
            });

            // Switched position
            a.KeyFrames.Add(new EasingDoubleKeyFrame() {
                Value = switchedValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(durationOfSwitchAnimation),
                EasingFunction = easingFunction
            });

            // Holding switched position
            a.KeyFrames.Add(new EasingDoubleKeyFrame() {
                Value = switchedValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(timeBetweenSwitch)
            });

            // Back to initial position
            a.KeyFrames.Add(new EasingDoubleKeyFrame() {
                Value = initialValue,
                KeyTime = a.KeyFrames.Last().KeyTime.TimeSpan.Add(durationOfSwitchAnimation),
                EasingFunction = easingFunction
            });


            return a;
        }

        private Storyboard _storyboard;

        private void InitializePeekAnimation() {
            Double normalHeight = this.PresentationCanvas.ActualHeight;

            if (this._storyboard != null) {
                this._storyboard.Stop();
            }


            this._storyboard = new Storyboard() {
                RepeatBehavior = RepeatBehavior.Forever
            };


            DoubleAnimationUsingKeyFrames a = GeneratePeekAnimation(
                normalHeight, this.PeekStartsOn,
                TimeSpan.FromSeconds(4));


            this._storyboard.Children.Add(a);

            Storyboard.SetTarget(a, this.TranslateContent);
            Storyboard.SetTargetProperty(a, "Y");

            this._storyboard.Begin();
        }
    }
}
