using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shell.Controls {

    public sealed partial class SettingsControl : UserControl {
        public String AppVersion { get; set; }
        public Shell.Models.SettingsModel Settings { get; set; }
        public Action<Shell.Models.SettingsModel> SettingsUpdated { get; set; }
        public ObservableCollection<NavLink> NavLinks { get; } = new ObservableCollection<NavLink>() {
            new NavLink() {
                Label = "Start",
                Symbol = Windows.UI.Xaml.Controls.Symbol.Home,
            },
            new NavLink() {
                Label = "Advanced",
                Symbol = Windows.UI.Xaml.Controls.Symbol.Admin
            },
            new NavLink() {
                Label = "About",
                Symbol = Windows.UI.Xaml.Controls.Symbol.OutlineStar
            },
        };

        public SettingsControl() {
            this.InitializeComponent();

            try {
                Package package = Package.Current;
                PackageVersion version = package.Id.Version;
                this.AppVersion = $"{version.Major}.{version.Minor}.{version.Revision}.{version.Build}";
            } catch { } // TODO: handle.
        }

        public void Control_OnReady() {
            // TODO
        }

        private void Nav_ItemClick(Object sender, ItemClickEventArgs e) {
            var children = this.SettingsView.Children;
            foreach(StackPanel child in children) {
                child.Visibility = Visibility.Collapsed;
            }

            var item = (NavLink)e.ClickedItem;
            ((StackPanel)this.SettingsView.FindName(item.Label)).Visibility = Visibility.Visible;
        }

        private void CornerRadius_Toggled(Object sender, RoutedEventArgs e) {
            if (this.Settings.CornerRadius == ((ToggleSwitch)sender).IsOn) return;
            this.Settings.CornerRadius = ((ToggleSwitch)sender).IsOn;

            if (this.SettingsUpdated == null) return;
            this.SettingsUpdated(this.Settings);
        }

        private void UseDesktopWallpaper_Toggled(Object sender, RoutedEventArgs e) {
            if (this.Settings.UseDesktopWallpaper == ((ToggleSwitch)sender).IsOn) return;
            this.Settings.UseDesktopWallpaper = ((ToggleSwitch)sender).IsOn;

            if (this.SettingsUpdated == null) return;
            this.SettingsUpdated(this.Settings);
        }

        private void EnableActionBar_Toggled(Object sender, RoutedEventArgs e) {
            if (this.Settings.EnableActionBar == ((ToggleSwitch)sender).IsOn) return;
            this.Settings.EnableActionBar = ((ToggleSwitch)sender).IsOn;

            if (this.SettingsUpdated == null) return;
            this.SettingsUpdated(this.Settings);
        }

        private async void SupportIAP_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args) {
            try {
                await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync("support", false);
            } catch { } // TODO: handle.
        }
    }

    public class NavLink {
        public String Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
