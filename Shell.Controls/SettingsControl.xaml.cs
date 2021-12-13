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
    public class SettingsModel {
        public Boolean CornerRadius { get; set; } = true;
    }

    public sealed partial class SettingsControl : UserControl {
        public String AppVersion { get; set; }
        public SettingsModel Settings { get; set; }
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

        private void Nav_ItemClick(Object sender, ItemClickEventArgs e) {
            var children = this.SettingsView.Children;
            foreach(StackPanel child in children) {
                child.Visibility = Visibility.Collapsed;
            }

            var item = (NavLink)e.ClickedItem;
            ((StackPanel)this.SettingsView.FindName(item.Label)).Visibility = Visibility.Visible;
        }
    }

    public class NavLink {
        public String Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
