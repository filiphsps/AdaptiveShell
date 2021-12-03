using Microsoft.Toolkit.Uwp.Notifications;
using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Management.Deployment;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Color = Windows.UI.Color;
using Size = Windows.Foundation.Size;
using TileSize = NotificationsVisualizerLibrary.TileSize;

namespace Shell.Pages {
    public class AppListItem {
        public String Key { get; set; }
        public BitmapImage Logo { get; set; }
        public String Title { get; set; }
        public AppListEntry Package { get; set; }
    }

    public class StartScreenItem {
        public String AppId { get; set; }
        public PreviewTile Tile { get; set; }
        public PreviewTileUpdater Updater { get; set; }
    }

    public sealed partial class StartPage : Page {
        public StartPage() {
            this.InitializeComponent();
        }

        private async void AppList_OnItemClick(Object sender, ItemClickEventArgs e) {
            var item = (AppListItem)e.ClickedItem;
            await item.Package.LaunchAsync();
        }

        private async void StartPage_OnLoaded(Object sender, RoutedEventArgs e) {
            var packageManager = new PackageManager();
            var packages = (IEnumerable<Windows.ApplicationModel.Package>)packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);

            var list = new List<AppListItem>();

            var liveTilesManager = new LiveTilesManager();
            List<TileModel> tiles = await liveTilesManager.GetLiveTiles();

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



            // Temp display all apps
            var startList = new List<StartScreenItem>();
            foreach (Package package in packages) {
                if (package.DisplayName.Length <= 0)
                    continue;

                if (package.IsFramework || package.IsResourcePackage || package.IsOptional || package.IsBundle)
                    continue;

                foreach (AppListEntry runtime in package.GetAppListEntries()) {
                    if (runtime.DisplayInfo.DisplayName.Length <= 0)
                        continue;

                    var tile = new NotificationsVisualizerLibrary.PreviewTile() {
                        TileSize = TileSize.Medium,
                        TileDensity = TileDensity.Tablet(),
                        DisplayName = runtime.DisplayInfo.DisplayName
                    };
                    
                    // TODO: do this properly
                    tile.VisualElements.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
                    tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                    tile.VisualElements.ShowNameOnWide310x150Logo = true;
                    tile.VisualElements.ShowNameOnSquare310x310Logo = true;

                    PreviewTileUpdater updater = tile.CreateTileUpdater();
                    startList.Add(new StartScreenItem {
                        AppId = runtime.AppUserModelId,
                        Tile = tile,
                        Updater = updater
                    });
                }
            }

            // TODO: only do this for actually pinned tiles
            foreach (TileModel tileData in tiles) {
                var tileNotification = new TileNotification(tileData.Payload);
                StartScreenItem tile = startList.FindLast(item => item.AppId == tileData.AppId);
                if (tile == null)
                    continue;
                
                PreviewTileUpdater updater = tile.Updater;
                updater.Update(tileNotification);
                await tile.Tile.UpdateAsync();
            }

            // Temp only show visible items
            startList.ForEach(item => {
                var tile = new Border() {
                    Background =  new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                    Child = item.Tile
                };

                tile.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 2);
                tile.SetValue(VariableSizedWrapGrid.RowSpanProperty, 2);
                tile.Margin = new Thickness(1.5);
                tile.CornerRadius = new CornerRadius(4);
                this.StartScreen.Children.Add(tile);
            });
        }
    }
}
