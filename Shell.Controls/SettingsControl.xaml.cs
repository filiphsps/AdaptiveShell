using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shell.Controls {

    public sealed partial class SettingsControl : UserControl {
        public String AppVersion { get; set; }
        public Shell.Models.SettingsModel Settings { get; set; }
        public Action<Shell.Models.SettingsModel> SettingsUpdated { get; set; }
        public ObservableCollection<NavLink> NavLinks { get; } = new ObservableCollection<NavLink>() {
            new NavLink() {
                Label = "Start",
                Symbol = Symbol.Home,
            },
            new NavLink() {
                Label = "Advanced",
                Symbol = Symbol.Admin
            },
            new NavLink() {
                Label = "About",
                Symbol = Symbol.OutlineStar
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

        private async void SupportIAP_Click(Hyperlink sender, HyperlinkClickEventArgs args) {
            try {
                await CurrentApp.RequestProductPurchaseAsync("support", false);
            } catch { } // TODO: handle.
        }
    }

    public class NavLink {
        public String Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
