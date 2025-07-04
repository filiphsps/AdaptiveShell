using System;
using System.Windows;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window {
        public Settings() {
            this.InitializeComponent();
        }

        private void SettingsControl_ChildChanged(Object sender, EventArgs e) {
            var windowsXamlHost = sender as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;

            if (windowsXamlHost == null)
                return; // TODO: handle this.

            var control = windowsXamlHost.GetUwpInternalObject() as global::Shell.Controls.SettingsControl;

            if (control == null)
                return; // TODO: handle this.

            control.Settings = ((Shell.Host.App)Application.Current).Settings;
            control.SettingsUpdated = (Shell.Models.SettingsModel settings) => {
                ((Shell.Host.App)Application.Current).Settings = settings;

                //if (((Shell.Host.App)Application.Current).OnSettingsUpdate != null)
                //    ((Shell.Host.App)Application.Current).OnSettingsUpdate(settings);
            };
            control.Control_OnReady();
        }
    }
}
