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

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : Window {
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
                var applicationManager = new Shell.LiveTilesAccessLibrary.ApplicationManager();
                await applicationManager.Initilize();
                control.ApplicationManager = applicationManager;

                control.ScreenHeight = this.Height;
                control.Height = this.Height;
                control.ScreenWidth = this.Width;
                control.Width = this.Width;

                control.ToggleVisibility = () => {
                    if (this.Visibility == Visibility.Visible)
                        this.Visibility = Visibility.Collapsed;
                    else
                        this.Visibility = Visibility.Visible;
                };

                control.Control_OnReady();
            } catch { }
        }
    }
}
