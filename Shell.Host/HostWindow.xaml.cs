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
        private StartScreen StartScreen;
        private ActionBar ActionBar;
        private StatusBar StatusBar;
        private Settings Settings;

        public HostWindow() {
            this.InitializeComponent();

            this.StartScreen = new StartScreen() {
                Topmost = Shell.Host.Features.StartScreenTopMost,
                ShowInTaskbar = false,
                OnExit = this.OnExit,
                OnSettings = this.OnSettings
            };
            StartScreen.Show();

            this.StatusBar = new StatusBar() {
                Topmost = Shell.Host.Features.StatusBarTopMost,
                ShowInTaskbar = false,
            };
            if (Shell.Host.Features.StatusBarEnabled)
                StatusBar.Show();

            this.ActionBar = new ActionBar() {
                Topmost = Shell.Host.Features.ActionBarTopMost,
                ShowInTaskbar = false
            };
            ActionBar.Show();

            ActionBar.HideStart += () => {
                this.StartScreen.Visibility = Visibility.Collapsed;
            };
            ActionBar.ToggleStart += () => {
                if (this.StartScreen.Visibility == Visibility.Visible)
                    this.StartScreen.Visibility = Visibility.Collapsed;
                else
                    this.StartScreen.Visibility = Visibility.Visible;
            };
        }

        private void OnSettings() {
            if (this.Settings == null) this.Settings = new Settings();
            if (!this.Settings.IsLoaded) this.Settings.Show();

            this.StartScreen.Visibility = Visibility.Collapsed;
            this.Settings.Focus();
            this.Settings.Closed += (Object? sender, EventArgs e) => {
                this.Settings = null;
            };
        }

        private void OnExit() {
            if(this.Settings != null)
                this.Settings.Close();

            this.ActionBar.Close();
            this.StartScreen.Close();
            this.StatusBar.Close();
            this.Close();
        }

        private void Window_Deactivated(Object sender, EventArgs e) {
            var window = (Window)sender;
            window.Topmost = true;
        }
    }
}
