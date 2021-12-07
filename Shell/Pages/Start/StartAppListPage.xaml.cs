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

namespace Shell.Pages {
    public class AppListItem {
        public String Key { get; set; }
        public BitmapImage Logo { get; set; }
        public String Title { get; set; }
        public AppListEntry Package { get; set; }
    }
    
    public sealed partial class StartAppListPage : Page {
        // public ObservableCollection<AppListItem> appCollection = new ObservableCollection<AppListItem>();

        private Double ScreenWidth;
        private Double ScreenHeight;

        public StartAppListPage() {
            this.InitializeComponent();
        }

        private void StartAppListPage_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.ScreenWidth = Window.Current.CoreWindow.Bounds.Width;
            this.ScreenHeight = Window.Current.CoreWindow.Bounds.Height;


            if (this.ScreenWidth <= 950) {
                this.AppListScrollViewer.Padding = new Thickness(0);
            } else {
                this.AppListScrollViewer.Padding = new Thickness(this.ScreenWidth * 0.05);
            }
        }

        private async void AppList_OnItemClick(Object sender, ItemClickEventArgs e) {
            var item = (AppListItem)e.ClickedItem;
            await item.Package.LaunchAsync();
        }

        private async void StartAppListPage_OnLoaded(Object sender, RoutedEventArgs e) {
            var packageManager = new PackageManager();
            var packages = (IEnumerable<Windows.ApplicationModel.Package>)packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);

            var list = new List<AppListItem>();

            foreach (Package package in packages) {
                if (package.DisplayName.Length <= 0)
                    continue;

                if (package.IsFramework || package.IsResourcePackage || package.IsOptional || package.IsBundle)
                    continue;

                IReadOnlyList<AppListEntry> entries = await package.GetAppListEntriesAsync();
                foreach (AppListEntry item in entries) {
                    var logo = new BitmapImage();

                    try {
                        RandomAccessStreamReference logoStream = item.DisplayInfo.GetLogo(new Size(250, 250));
                        IRandomAccessStreamWithContentType logoStreamSource = await logoStream.OpenReadAsync();
                        logo.SetSource(logoStreamSource);
                    } catch { }

                    list.Add(new AppListItem {
                        Key = item.DisplayInfo.DisplayName.Substring(0, 1).ToUpper(),
                        Logo = logo,
                        Title = item.DisplayInfo.DisplayName,
                        Package = item
                    });
                }
            }
            IEnumerable<IGrouping<String, AppListItem>> result = from t in list group t by t.Key;
            this.ApplicationList.Source = result.OrderBy(item => item.Key);
        }
    }
}
