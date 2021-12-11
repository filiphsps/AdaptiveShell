using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Shell {
    public sealed partial class SplashPage : Page {
        private System.Type _page;

        public SplashPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this._page = (System.Type)e.Parameter;
        }

        private async void SplashPage_OnLoaded(Object sender, RoutedEventArgs e) {
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            /* if (!System.Diagnostics.Debugger.IsAttached) {
                var view = ApplicationView.GetForCurrentView();
                view.TryEnterFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            } else {
            } */
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;

            var liveTilesManager = new LiveTilesAccessLibrary.ApplicationManager();
            await liveTilesManager.Initilize();

            // TODO
            this.Frame.Navigate(this._page, liveTilesManager, new EntranceNavigationTransitionInfo());
            this.Frame.BackStack.Remove(this.Frame.BackStack.Last());
        }
    }
}
