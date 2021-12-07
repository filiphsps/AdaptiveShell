using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Size = Windows.Foundation.Size;

namespace Shell.Pages {
    public sealed partial class StartPage : Page {
        private Double ScreenWidth;
        private Double ScreenHeight;

        public StartPage() {
            this.InitializeComponent();

            this.StartPage_SizeChanged(null, null);

            this.StartScreenLayout.SourcePageType = typeof(Pages.StartLiveTilesPage);
            this.AppsListLayout.SourcePageType = typeof(Pages.StartAppListPage);
        }

        private void StartPage_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.ScreenWidth = Window.Current.CoreWindow.Bounds.Width;
            this.ScreenHeight = Window.Current.CoreWindow.Bounds.Height;

            if (this.ScreenWidth <= 950) {
                this.StartScreenLayout.Height = Double.NaN;
                this.StartScreenLayout.Width = this.ScreenWidth;
                this.AppsListLayout.Height = Double.NaN;
                this.AppsListLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Center;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Center;
                this.AppsListLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Horizontal;
            } else {
                this.StartScreenLayout.Height = this.ScreenHeight;
                this.StartScreenLayout.Width = Double.NaN;
                this.StartScreenLayout.Height = this.ScreenHeight;
                this.AppsListLayout.Width = Double.NaN;

                this.Start.Orientation = Orientation.Vertical;
                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Center;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        private async void StartPage_OnLoaded(Object sender, RoutedEventArgs e) {
            // Set wallpaper
            var background = await Shell.PersonalizationLibrary.BackgroundImageManager.GetBackgroundImage();
            if (background != null)
                this.Background = new ImageBrush() {
                    ImageSource = background,
                    Stretch = Stretch.UniformToFill
                };
        }

        private void StartScreenLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.StartPage_SizeChanged(null, null);
        }

        private void AppsListLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.StartPage_SizeChanged(null, null);
        }

        private void ScrollViewer_ViewChanging(Object sender, ScrollViewerViewChangingEventArgs e) {
            Int32 MAX_DARK = 65;

            if (this.ScreenWidth <= 950) {
                this.Root.Background = new SolidColorBrush() {
                    Color = Color.FromArgb(Convert.ToByte(
                        (e.NextView.HorizontalOffset / ((ScrollViewer)sender).ViewportWidth) * MAX_DARK
                    ), 0, 0, 0)
                };
            } else {
                this.Root.Background = new SolidColorBrush() {
                    Color = Color.FromArgb(Convert.ToByte(
                        (e.NextView.VerticalOffset / ((ScrollViewer)sender).ViewportHeight) * MAX_DARK
                    ), 0, 0, 0)
                };
            }
        }
    }
}
