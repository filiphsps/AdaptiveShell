using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.Linq;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shell.Pages {
    public class StartScreenItem {
        public String AppId { get; set; }
        public String DisplayName { get; set; }
        public PreviewTile LiveTile { get; set; }
        public TileSize Size { get; set; }

        public TileDensity Density { get; set; } = TileDensity.Mobile(1.75);

        public Int32 RowSpawn {
            get {
                switch (this.Size) {
                    case TileSize.Large:
                        return 4;
                    case TileSize.Wide:
                    case TileSize.Medium:
                        return 2;
                    case TileSize.Small:
                    default:
                        return 1;
                }
            }
        }

        public Int32 ColumnSpawn {
            get {
                switch (this.Size) {
                    case TileSize.Large:
                        return 4;
                    case TileSize.Wide:
                        return 4;
                    case TileSize.Medium:
                        return 2;
                    case TileSize.Small:
                    default:
                        return 1;
                }
            }
        }

        public Boolean IsSmall { get => this.Size == TileSize.Small; }
        public Boolean IsMedium { get => this.Size == TileSize.Medium; }
        public Boolean IsWide { get => this.Size == TileSize.Wide; }
        public Boolean IsLarge { get => this.Size == TileSize.Large; }
    }

    public sealed partial class StartLiveTilesPage : Page {
        public ObservableCollection<StartScreenItem> tileCollection = new ObservableCollection<StartScreenItem>();
        public List<TileModel> tileUpdates = new List<TileModel>();

        public StartLiveTilesPage() {
            this.InitializeComponent();
        }

        private async void StartLiveTilesPage_OnLoaded(Object sender, RoutedEventArgs e) {
            var packageManager = new PackageManager();
            var packages = packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);

            try {
                var liveTilesManager = new LiveTilesManager();
                this.tileUpdates = await liveTilesManager.GetLiveTiles();
            } catch {
                // TODO
            }

            // Temp display all apps
            packages.ToList().ForEach ((package) => {
                _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    if (package.DisplayName.Length <= 0)
                        return;

                    if (package.IsFramework || package.IsResourcePackage || package.IsOptional || package.IsBundle)
                        return;

                    foreach (AppListEntry runtime in package.GetAppListEntries()) {
                        if (runtime.DisplayInfo.DisplayName.Length <= 0)
                            continue;

                        var gridItem = new StartScreenItem {
                            AppId = runtime.AppUserModelId,
                            Size = TileSize.Medium,
                            DisplayName = runtime.DisplayInfo.DisplayName
                        };
                        this.tileCollection.Add(gridItem);
                    }
                });
            });
        }

        private async void LiveTile_Loaded(Object sender, RoutedEventArgs e) {
            var item = (StartScreenItem)((Grid)sender).DataContext;
            var tile = (PreviewTile)((Grid)sender).Children[0];
            item.LiveTile = tile;

            var currentTileUpdates = this.tileUpdates.FindAll(update => update.AppId == item.AppId);
            if (currentTileUpdates.Count <= 0) {
                await tile.UpdateAsync();
                return;
            }

            PreviewTileUpdater tileUpdater = tile.CreateTileUpdater();

            // FIXME: Queue
            foreach (var update in currentTileUpdates) {
                tileUpdater.Update(new TileNotification(update.Payload));
            }

            PreviewBadgeUpdater badgeUpdater = tile.CreateBadgeUpdater();

            await tile.UpdateAsync();
        }

        private async void LiveTile_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            // TODO
            var item = (StartScreenItem)((Grid)sender).DataContext;
        }

        private async void LiveTileContext_Click(Object sender, RoutedEventArgs e) {
            var item = (StartScreenItem)((ToggleMenuFlyoutItem)sender).DataContext;
            var tile = item.LiveTile;

            switch (((MenuFlyoutItem)sender).Name) {
                case "SmallOpt":
                    item.Size = TileSize.Small;
                    break;
                case "MediumOpt":
                    item.Size = TileSize.Medium;
                    break;
                case "WideOpt":
                    item.Size = TileSize.Wide;
                    break;
                case "LargeOpt":
                    item.Size = TileSize.Large;
                    break;
            }

            // FIXME: figure out why checkmark doesn't update

            tile.UpdateLayout();
            await tile.UpdateAsync();
        }
    }
}
