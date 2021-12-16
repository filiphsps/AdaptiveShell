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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window {
        private Splash SplashWindow;
        private StartScreen StartScreenWindow;
        private ActionBar ActionBarWindow;
        private StatusBar StatusBarWindow;
        private Settings? SettingsWindow;

        public HostWindow() {
            this.InitializeComponent();
            var settings = ((Shell.Host.App)Application.Current).Settings;

            Functions.DisableWPFTabletSupport();

            this.SplashWindow = new Splash() {
                Topmost = true,
                ShowInTaskbar = false
            };
            this.SplashWindow.Show();

            this.StartScreenWindow = new StartScreen() {
                Topmost = Shell.Host.Features.StartScreenTopMost,
                ShowInTaskbar = false,
                OnExit = this.OnExit,
                OnSettings = this.OnSettings,
                Visibility = Visibility.Collapsed
            };
            this.StartScreenWindow.DoneLoaded += () => {
                this.SplashWindow.Close();
            };
            this.StartScreenWindow.Show();

            this.StatusBarWindow = new StatusBar() {
                Topmost = Shell.Host.Features.StatusBarTopMost,
                ShowInTaskbar = false,
            };
            if (settings.EnableStatusBar)
                this.StatusBarWindow.Show();

            this.ActionBarWindow = new ActionBar() {
                Topmost = Shell.Host.Features.ActionBarTopMost,
                ShowInTaskbar = false
            };
            if (settings.EnableActionBar)
                this.ActionBarWindow.Show();

            this.ActionBarWindow.HideStart += () => {
                this.StartScreenWindow.Visibility = Visibility.Collapsed;
            };

            this.ActionBarWindow.ToggleStart += () => {
                if (this.StartScreenWindow.Visibility == Visibility.Visible)
                    this.StartScreenWindow.Visibility = Visibility.Collapsed;
                else
                    this.StartScreenWindow.Visibility = Visibility.Visible;
            };
        }

        private void OnSettings() {
            if (this.SettingsWindow == null) this.SettingsWindow = new Settings();
            if (!this.SettingsWindow.IsLoaded) this.SettingsWindow.Show();

            this.StartScreenWindow.Visibility = Visibility.Collapsed;
            this.SettingsWindow.Focus();
            this.SettingsWindow.Closed += (Object? sender, EventArgs e) => {
                this.SettingsWindow = null;
            };
        }

        private void OnExit() {
            if (this.SplashWindow != null)
                this.SplashWindow.Close();

            if (this.SettingsWindow != null)
                this.SettingsWindow.Close();

            this.ActionBarWindow.Close();
            this.StartScreenWindow.Close();
            this.StatusBarWindow.Close();
            this.Close();
        }

        private void Window_Deactivated(Object sender, EventArgs e) {
            var window = (Window)sender;
            window.Topmost = true;
        }
    }
}
