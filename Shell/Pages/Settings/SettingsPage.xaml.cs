using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shell.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        public SettingsPage() {
            this.InitializeComponent();

            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.LoadPage((String)e.Parameter);
        }

        private void LoadPage(String page) {
            switch (page.ToLower()) {
                case "personalization":
                    this.SettingsFrame.SourcePageType = typeof(Pages.SettingsPersonalizationPage);
                    break;
                case "advanced":
                    this.SettingsFrame.SourcePageType = typeof(Pages.SettingsAdvancedPage);
                    break;
                default:
                    this.SettingsFrame.SourcePageType = null;
                    break;
            }
        }

        private void NavigationList_OnItemClick(Object sender, ItemClickEventArgs e) {
            String page = ((StackPanel)e.ClickedItem).Name;
            this.LoadPage(page.Split("Item")[0].ToLower());
        }
    }
}
