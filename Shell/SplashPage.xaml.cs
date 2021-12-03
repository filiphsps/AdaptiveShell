using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Shell {
    public sealed partial class SplashPage : Page {
        public SplashPage() {
            this.InitializeComponent();
        }

        private void SplashPage_OnLoaded(Object sender, RoutedEventArgs e) {
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            // TODO
            this.Frame.Navigate(typeof(Pages.StartPage));
            this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
        }
    }
}
