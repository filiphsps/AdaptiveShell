using System;
using System.Collections.Generic;
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
    public sealed partial class ActionBarControl : UserControl {
        public Windows.UI.Xaml.GridLength ActionBarItemHeight { get; set; } = new Windows.UI.Xaml.GridLength(48);
        public Windows.UI.Xaml.GridLength ActionBarItemWidth { get; set; } = new Windows.UI.Xaml.GridLength(48);

        public Action OnBack { get; set; }
        public Action OnStart { get; set; }
        public Action OnSearch { get; set; }

        public ActionBarControl() {
            this.InitializeComponent();
        }

        private void BackBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.OnBack();
        }

        private void StartBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.OnStart();
        }

        private void SearchBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.OnSearch();
        }
    }
}
