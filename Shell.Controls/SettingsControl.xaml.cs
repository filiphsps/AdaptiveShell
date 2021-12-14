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

            Package package = Package.Current;
            PackageVersion version = package.Id.Version;
            this.AppVersion = $"{version.Major}.{version.Minor}.{version.Revision}.{version.Build}";
        }

        public void Control_OnReady() {
            this.IsEnabled = true;
        }

        private void Nav_ItemClick(Object sender, ItemClickEventArgs e) {
            var children = this.SettingsView.Children;
            foreach(StackPanel child in children) {
                child.Visibility = Visibility.Collapsed;
            }

            var item = (NavLink)e.ClickedItem;
            ((StackPanel)this.SettingsView.FindName(item.Label)).Visibility = Visibility.Visible;
        }

        private void ToggleSwitch_Toggled(Object sender, RoutedEventArgs e) {
            if (this.SettingsUpdated == null) return;

            this.SettingsUpdated(this.Settings);
        }
    }

    public class NavLink {
        public String Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
