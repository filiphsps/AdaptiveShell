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
        public ApplicationManager ApplicationManager { get; set; }
        public Action ToggleVisibility { get; set; }
        public Action OnExit { get; set; }
        public Action OnSettings { get; set; }
        public Action OnFocusLost { get; set; }
        public Double ScreenWidth;
        public Double ScreenHeight;

        public StartScreenControl() {
            this.InitializeComponent();
        }

        private async void Control_OnLoaded(Object sender, RoutedEventArgs e) {
            // Set wallpaper
            BitmapImage background = await Shell.PersonalizationLibrary.BackgroundImageManager.GetBackgroundImage();
            if (background != null)
                this.Root.Background = new ImageBrush() {
                    ImageSource = background,
                    Stretch = Stretch.UniformToFill
                };

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
        }

        public void Control_OnReady() {
            Debug.WriteLine("StartScreenControl OnReady!");

            if (this.ApplicationManager == null) return;

            this.LiveTilesLayout.ScreenHeight = this.ScreenHeight;
            this.LiveTilesLayout.ScreenWidth = this.ScreenWidth;
            this.LiveTilesLayout.ItemsSource = this.ApplicationManager.LiveTiles;
            this.LiveTilesLayout.Control_OnReady();

            this.AppsListLayout.ScreenHeight = this.ScreenHeight;
            this.AppsListLayout.ScreenWidth = this.ScreenWidth;
            this.AppsListLayout.ItemsSource = this.ApplicationManager.LiveTiles;
            this.AppsListLayout.Control_OnReady();

            this.Control_SizeChanged(null, null);
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.LiveTilesLayout.ScreenHeight = this.ScreenHeight;
            this.LiveTilesLayout.ScreenWidth = this.ScreenWidth;
            this.AppsListLayout.ScreenHeight = this.ScreenHeight;
            this.AppsListLayout.ScreenWidth = this.ScreenWidth;

            if (this.ScreenWidth <= 950) {
                this.StartHeaderToolbar.Padding = new Thickness(0);
                this.StartFooterToolbar.Padding = new Thickness(0);
                this.AppsHeaderToolbar.Padding = new Thickness(0);
                this.AppsFooterToolbar.Padding = new Thickness(0);

                this.StartScreenLayout.Height = Double.NaN;
                this.StartScreenLayout.Width = this.ScreenWidth;
                this.AppsScreenLayout.Height = Double.NaN;
                this.AppsScreenLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.AppsListLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Horizontal;
                this.Apps.Orientation = Orientation.Horizontal;
            } else {
                Double padding = this.ScreenWidth * 0.025;
                this.StartHeaderToolbar.Padding = new Thickness(padding, this.ScreenHeight * 0.05, padding, 0);
                this.StartFooterToolbar.Padding = new Thickness(padding, 0, padding, this.ScreenHeight * 0.05);
                this.AppsHeaderToolbar.Padding = new Thickness(padding, this.ScreenHeight * 0.05, padding, 0);
                this.AppsFooterToolbar.Padding = new Thickness(padding, 0, padding, this.ScreenHeight * 0.05);

                this.StartScreenLayout.Height = this.ScreenHeight - (this.StartHeaderToolbar.ActualHeight + this.StartFooterToolbar.ActualHeight) + 25;
                this.StartScreenLayout.Width = this.ScreenWidth;
                // Hack to make scrollbar work
                this.AppsListLayout.Height = this.ScreenHeight - (this.AppsHeaderToolbar.ActualHeight + this.AppsFooterToolbar.ActualHeight) - 125;
                this.AppsListLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.AppsListLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Vertical;
                this.Apps.Orientation = Orientation.Vertical;
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
