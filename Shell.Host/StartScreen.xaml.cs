using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Toolkit.Wpf.UI.XamlHost;
using Microsoft.Win32;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : Window {
        public Action OnExit { get; set; }
        public Action OnSettings { get; set; }
        public Action DoneLoaded { get; set; }
        private System.Windows.Threading.DispatcherTimer DispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public StartScreen() {
            this.InitializeComponent();

            this.Height = Functions.STARTSCREEN_HEIGHT;
            this.Left = 0;
            this.Top = Functions.STATUSBAR_HEIGHT;

            // var frame = (Windows.UI.Xaml.Controls.Frame)this.MainFrame.Child;
            // frame.Navigate(typeof(Shell.SplashPage), null);
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e) {
            if (!Features.StartScreenTopMost) return;

            var wndHelper = new WindowInteropHelper(this);
            Int32 exStyle = (Int32)WinAPI.GetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (Int32)WinAPI.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WinAPI.SetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private async void StartScreenControl_ChildChanged(Object sender, EventArgs e) {
            var windowsXamlHost = sender as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;

            if (windowsXamlHost == null)
                return; // TODO: handle this.

            var control = windowsXamlHost.GetUwpInternalObject() as global::Shell.Controls.StartScreenControl;

            if (control == null)
                return; // TODO: handle this.

            // Temp
            try {
                var settings = ((Shell.Host.App)Application.Current).Settings;
                ((Shell.Host.App)Application.Current).OnSettingsUpdate += (Shell.Models.SettingsModel settings) => {
                    control.Settings = settings;
                    control.Control_OnReady();
                };

                var applicationManager = new Shell.LiveTilesAccessLibrary.ApplicationManager();
                await applicationManager.Initilize();
                control.ApplicationManager = applicationManager;

                this.DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                this.DispatcherTimer.Tick += (Object? tickSender, EventArgs tickE) => {
                    _ = applicationManager.UpdateLiveTiles();
                };
                this.DispatcherTimer.Interval = new TimeSpan(0, 15, 0);
                this.DispatcherTimer.Start();

                if (settings.UseDesktopWallpaper) {
                    String? userWallpaper = (String)Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false).GetValue("WallPaper");
                    if (userWallpaper != null) {
                        StorageFile background = await StorageFile.GetFileFromPathAsync(userWallpaper);

                        using (IRandomAccessStream fileStream = await background.OpenAsync(Windows.Storage.FileAccessMode.Read)) {
                            // Set the image source to the selected bitmap
                            var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                            await bitmapImage.SetSourceAsync(fileStream);
                            control.Wallpaper = bitmapImage;
                        }
                    }
                } else {
                    control.Wallpaper = null;
                }

                control.ScreenHeight = this.Height;
                control.Height = this.Height;
                control.ScreenWidth = this.Width;
                control.Width = this.Width;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                control.OnFocusLost = () => this.Window_LostFocus(null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                control.OnExit = this.OnExit;
                control.OnSettings = this.OnSettings;
                control.Settings = settings;

                control.ToggleVisibility = () => {
                    if (this.Visibility == Visibility.Visible)
                        this.Visibility = Visibility.Collapsed;
                    else
                        this.Visibility = Visibility.Visible;
                };

                control.Control_OnReady();
                if (this.DoneLoaded != null) this.DoneLoaded();
                this.Visibility = Visibility.Visible;
            } catch { }
        }

        private void Window_SizeChanged(Object sender, SizeChangedEventArgs e) {
            var windowsXamlHost = this.StartScreenControl as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;
            var control = windowsXamlHost.GetUwpInternalObject() as global::Shell.Controls.StartScreenControl;
            var settings = ((Shell.Host.App)Application.Current).Settings;

            if (control == null) return;

            control.ScreenHeight = this.Height;
            control.Height = this.Height;
            control.ScreenWidth = this.Width;
            control.Width = this.Width;
            control.Settings = settings;

            this.Height = Functions.STARTSCREEN_HEIGHT;
            this.Left = 0;
            this.Top = Functions.STATUSBAR_HEIGHT;
        }

        // Handle alt-tab
        private void Window_LostFocus(Object sender, RoutedEventArgs e) {
            this.Visibility = Visibility.Collapsed;
        }

        private void Window_Closed(Object sender, EventArgs e) {
            this.DispatcherTimer.Stop();
        }
    }
}
