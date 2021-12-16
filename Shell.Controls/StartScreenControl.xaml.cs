using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Shell.Controls {
    public sealed partial class StartScreenControl : UserControl {
        public Shell.Models.SettingsModel Settings { get; set; }
        public ApplicationManager ApplicationManager { get; set; }
        public Action ToggleVisibility { get; set; }
        public Action OnExit { get; set; }
        public Action OnSettings { get; set; }
        public Action OnFocusLost { get; set; }
        public BitmapImage Wallpaper { get; set; }
        public Double ScreenWidth = 1920;
        public Double ScreenHeight = 1080;

        public StartScreenControl() {
            this.InitializeComponent();
        }

        private async void Control_OnLoaded(Object sender, RoutedEventArgs e) {
            // Set profile
            try {
                System.Collections.Generic.IReadOnlyList<User> users = await Windows.System.User.FindAllAsync();
                User user = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
                                            p.Type == UserType.LocalUser).FirstOrDefault();

                Windows.Storage.Streams.IRandomAccessStreamReference userPicure = await user.GetPictureAsync(Windows.System.UserPictureSize.Size208x208);
                var contact = new Windows.ApplicationModel.Contacts.Contact { };
                contact.SourceDisplayPicture = userPicure;
                this.ProfilePicture.Contact = contact;
            } catch { }

            this.Control_SizeChanged(null, null);
        }

        public void Control_OnReady() {
            Debug.WriteLine("[StartScreenControl] OnReady!");
            Debug.WriteLine($"[StartScreenControl] Width: {this.ScreenWidth}, Height: {this.ScreenHeight}");

            // Set wallpaper
            if (this.Wallpaper != null)
                this.Root.Background = new ImageBrush() {
                    ImageSource = this.Wallpaper,
                    Stretch = Stretch.UniformToFill
                };
            else
                this.Root.Background = new SolidColorBrush() {
                    Color = (Windows.UI.Color)Application.Current.Resources["SystemAccentColorLight1"]
                };

            this.LiveTilesLayout.ScreenHeight = this.ScreenHeight;
            this.LiveTilesLayout.ScreenWidth = this.ScreenWidth;
            this.LiveTilesLayout.ItemsSource = this.ApplicationManager.LiveTiles;
            this.LiveTilesLayout.ToggleVisibility = this.ToggleVisibility;
            this.LiveTilesLayout.Settings = this.Settings;
            this.LiveTilesLayout.Control_OnReady();

            this.AppsListLayout.ScreenHeight = this.ScreenHeight;
            this.AppsListLayout.ScreenWidth = this.ScreenWidth;
            this.AppsListLayout.ItemsSource = this.ApplicationManager.LiveTiles;
            this.AppsListLayout.ToggleVisibility = this.ToggleVisibility;
            this.AppsListLayout.Settings = this.Settings;
            this.AppsListLayout.Control_OnReady();


            this.Control_SizeChanged(null, null);
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            if (this.ScreenWidth <= 950) {
            } else {
                Double padding = this.ScreenWidth * 0.025;
                this.StartHeaderToolbar.Padding = new Thickness(padding, this.ScreenHeight * 0.05, padding, 0);
                this.StartFooterToolbar.Padding = new Thickness(padding, 0, padding, this.ScreenHeight * 0.05);
                this.AppsHeaderToolbar.Padding = new Thickness(padding, this.ScreenHeight * 0.05, padding, 0);
                this.AppsFooterToolbar.Padding = new Thickness(padding, 0, padding, this.ScreenHeight * 0.05);

                this.StartScreenLayout.Height = this.ScreenHeight;
                this.StartScreenLayout.Width = this.ScreenWidth;
                // Hack to make scrollbar work
                this.AppsListLayout.Height = this.ScreenHeight - (this.AppsHeaderToolbar.ActualHeight + this.AppsFooterToolbar.ActualHeight + 50);
                this.AppsListLayout.Width = this.ScreenWidth;
            }

            // Force update
            this.Root.UpdateLayout();
        }

        private void ScrollViewer_ViewChanging(Object sender, ScrollViewerViewChangingEventArgs e) {
            Int32 MAX_DARK = 175;

            try {
                if (this.ScreenWidth <= 950) {
                    if (((ScrollViewer)sender).HorizontalOffset == e.NextView.HorizontalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.HorizontalOffset / ((ScrollViewer)sender).ViewportWidth) * MAX_DARK
                        ), 0, 0, 0)
                    };
                } else {
                    if (((ScrollViewer)sender).VerticalOffset == e.NextView.VerticalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.VerticalOffset / ((ScrollViewer)sender).ViewportHeight) * MAX_DARK
                        ), 0, 0, 0)
                    };
                }
            } catch { }
        }

        private void ExitBtn_Click(Object sender, RoutedEventArgs e) {
            if (this.OnExit == null) return;
            this.OnExit();
        }

        private void SettingsBtn_Click(Object sender, RoutedEventArgs e) {
            if (this.OnSettings == null) return;
            this.OnSettings();
        }

        private void UserControl_FocusDisengaged(Control sender, FocusDisengagedEventArgs args) {
            if (this.OnFocusLost == null) return;

            this.OnFocusLost();
        }

        private void AllAppsBtn_Click(Object sender, RoutedEventArgs e) {
            if (this.ScreenWidth <= 950)
                this.RootScroll.ChangeView(this.RootScroll.ScrollableWidth, null, null);
            else
                this.RootScroll.ChangeView(null, this.RootScroll.ScrollableHeight, null);
        }

        private void StartBtn_Click(Object sender, RoutedEventArgs e) {
            if (this.ScreenWidth <= 950)
                this.RootScroll.ChangeView(0, null, null);
            else
                this.RootScroll.ChangeView(null, 0, null);
        }
    }
}
