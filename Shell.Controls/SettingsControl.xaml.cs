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
            new NavLink() { Label = "People", Symbol = Windows.UI.Xaml.Controls.Symbol.People  },
            new NavLink() { Label = "Globe", Symbol = Windows.UI.Xaml.Controls.Symbol.Globe },
            new NavLink() { Label = "Message", Symbol = Windows.UI.Xaml.Controls.Symbol.Message },
            new NavLink() { Label = "Mail", Symbol = Windows.UI.Xaml.Controls.Symbol.Mail },
        };

        public SettingsControl() {
            this.InitializeComponent();
        }
    }

    public class NavLink {
        public String Label { get; set; }
        public Symbol Symbol { get; set; }
    }
}
