using System;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Shell {
    public sealed partial class SplashPage : Page {
        public SplashPage() {
            this.InitializeComponent();
        }

        private void SplashPage_OnLoaded(Object sender, RoutedEventArgs e) {
            // TODO
            this.Frame.Navigate(typeof(Pages.StartPage));
            this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
        }
    }
}
