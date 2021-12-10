using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static Shell.Pages.StartPage;

namespace Shell.Pages {
    public class AppListItem {
        public String AppId { get; set; }
        public String Key { get; set; }
        public BitmapImage Logo { get; set; }
        public String Title { get; set; }
        public AppListEntry Package { get; set; }
    }
    
    public sealed partial class StartAppListPage : Page {
        private Double ScreenWidth;
        private Double ScreenHeight;
        private AppListParameters Arguments;

        public StartAppListPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.Arguments = (AppListParameters)e.Parameter;
        }

        private void StartAppListPage_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.ScreenWidth = Window.Current.CoreWindow.Bounds.Width;
            this.ScreenHeight = Window.Current.CoreWindow.Bounds.Height;


            if (this.ScreenWidth <= 950) {
                this.AppListScrollViewer.Padding = new Thickness(0);
                this.AppListScrollViewer.Margin = new Thickness(0);
                this.BackToStartBtn.Padding = new Thickness(this.ScreenWidth * 0.05);

                this.AppListScrollViewer.SetValue(Grid.RowProperty, 0);
                this.BackToStartBtn.SetValue(Grid.RowProperty, 1);
            } else {
                this.AppListScrollViewer.Padding = new Thickness(this.ScreenWidth * 0.025);
                this.AppListScrollViewer.Margin = new Thickness(0, ((this.ScreenWidth * 0.025) * -1), 0, 0);
                this.BackToStartBtn.Padding = new Thickness(this.ScreenWidth * 0.05, this.ScreenWidth * 0.025, this.ScreenWidth * 0.025, 14);

                this.AppListScrollViewer.SetValue(Grid.RowProperty, 1);
                this.BackToStartBtn.SetValue(Grid.RowProperty, 0);
            }
        }

        private async void AppList_OnItemClick(Object sender, ItemClickEventArgs e) {
            var item = (AppListItem)e.ClickedItem;
            await item.Package.LaunchAsync();
        }

        private async void StartAppListPage_OnLoaded(Object sender, RoutedEventArgs e) {
            var packages = this.Arguments.LiveTilesManager.Packages;
            var list = new List<AppListItem>();

            foreach (Package package in packages) {
                if (package.DisplayName.Length <= 0)
                    continue;

                if (package.IsFramework || package.IsResourcePackage || package.IsOptional || package.IsBundle)
                    continue;

                IReadOnlyList<AppListEntry> entries = await package.GetAppListEntriesAsync();
                foreach (AppListEntry runtime in entries) {
                    if (runtime.DisplayInfo.DisplayName.Length <= 0 || runtime.DisplayInfo.DisplayName == "NoUIEntryPoints-DesignMode")
                        continue;

                    var logo = new BitmapImage();

                    try {
                        RandomAccessStreamReference logoStream = runtime.DisplayInfo.GetLogo(new Size(250, 250));
                        IRandomAccessStreamWithContentType logoStreamSource = await logoStream.OpenReadAsync();
                        logo.SetSource(logoStreamSource);
                    } catch { }

                    list.Add(new AppListItem {
                        AppId = runtime.AppUserModelId,
                        Key = runtime.DisplayInfo.DisplayName.Substring(0, 1).ToUpper(),
                        Logo = logo,
                        Title = runtime.DisplayInfo.DisplayName,
                        Package = runtime
                    });
                }
            }
            IEnumerable<IGrouping<String, AppListItem>> result = from t in list group t by t.Key;
            this.ApplicationList.Source = result.OrderBy(item => item.Key);
        }

        private void PinToStart_Click(Object sender, RoutedEventArgs e) {
            this.Arguments.LiveTilesManager.ToggleIsPinned(
                ((AppListItem)((MenuFlyoutItem)sender).DataContext).AppId
            );
            this.Arguments.StartScreenBtnCallback();
        }

        private void BackToStartBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.Arguments.StartScreenBtnCallback();
        }
    }
}
