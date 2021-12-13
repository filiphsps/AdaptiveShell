using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public ObservableCollection<NavLink> NavLinks { get; } = new ObservableCollection<NavLink>() {
            new NavLink() {
                Label = "Start",
                Symbol = Windows.UI.Xaml.Controls.Symbol.Home
            },
            new NavLink() {
                Label = "Advanced",
                Symbol = Windows.UI.Xaml.Controls.Symbol.Setting
            },
        };

        public SettingsControl() {
            this.InitializeComponent();
        }

        private void Nav_ItemClick(Object sender, ItemClickEventArgs e) {
            return;
        }
    }

    public class NavLink {
        public String Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
