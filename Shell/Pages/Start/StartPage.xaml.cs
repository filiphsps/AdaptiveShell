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
using Windows.UI.Xaml.Navigation;
using Size = Windows.Foundation.Size;

namespace Shell.Pages {
    public sealed partial class StartPage : Page {
        private Double ScreenWidth;
        private Double ScreenHeight;

        public class StartScrenParameters {
            public Action AllAppsBtnCallback { get; set; }
            public LiveTilesAccessLibrary.ApplicationManager LiveTilesManager { get; set; }
        }

        public class AppListParameters {
            public Action StartScreenBtnCallback { get; set; }
            public LiveTilesAccessLibrary.ApplicationManager LiveTilesManager { get; set; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            var liveTilesManager = (LiveTilesAccessLibrary.ApplicationManager)e.Parameter;

            this.StartScreenLayout.Navigate(typeof(Pages.StartLiveTilesPage), new StartScrenParameters() {
                LiveTilesManager = liveTilesManager,
                AllAppsBtnCallback = () => {
                    if (this.ScreenWidth <= 950) {
                        this.RootScroll.ChangeView(this.ScreenWidth, null, null);
                    } else {
                        this.RootScroll.ChangeView(null, this.ScreenHeight, null);
                    }
                }
            }, null);

            this.AppsListLayout.Navigate(typeof(Pages.StartAppListPage), new AppListParameters() {
                LiveTilesManager = liveTilesManager,
                StartScreenBtnCallback = () => {
                    if (this.ScreenWidth <= 950) {
                        this.RootScroll.ChangeView(0, null, null);
                    } else {
                        this.RootScroll.ChangeView(null, 0, null);
                    }
                }
            }, null);
        }

        public StartPage() {
            this.InitializeComponent();

            this.StartPage_SizeChanged(null, null);
        }

        private void StartPage_SizeChanged(Object sender, SizeChangedEventArgs e) {
            try {
                this.ScreenWidth = Window.Current.CoreWindow.Bounds.Width;
                this.ScreenHeight = Window.Current.CoreWindow.Bounds.Height;

                if (this.ScreenWidth <= 950) {
                    this.StartScreenLayout.Height = Double.NaN;
                    this.StartScreenLayout.Width = this.ScreenWidth;
                    this.AppsListLayout.Height = Double.NaN;
                    this.AppsListLayout.Width = this.ScreenWidth;

                    this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Center;
                    this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Top;
                    this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Center;
                    this.AppsListLayout.VerticalAlignment = VerticalAlignment.Top;

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
            } catch { }
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
            Int32 MAX_DARK = 85;

            try {
                if (this.ScreenWidth <= 950) {
                    if (((ScrollViewer)sender).HorizontalOffset == e.NextView.HorizontalOffset) return;

                    this.Root.Background = new SolidColorBrush() {
                        Color = Color.FromArgb(Convert.ToByte(
                            (e.NextView.HorizontalOffset / ((ScrollViewer)sender).ViewportWidth) * MAX_DARK
                        ), 0, 0, 0)
                    };
                } else {
                    if (((ScrollViewer)sender).VerticalOffset == e.NextView.VerticalOffset) return;

                    this.Root.Background = new SolidColorBrush() {
                        Color = Color.FromArgb(Convert.ToByte(
                            (e.NextView.VerticalOffset / ((ScrollViewer)sender).ViewportHeight) * MAX_DARK
                        ), 0, 0, 0)
                    };
                }
            } catch { }
        }
    }
}
