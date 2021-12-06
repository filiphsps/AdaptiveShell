using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shell.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartLiveTilesPage : Page {
        public StartLiveTilesPage() {
            this.InitializeComponent();
        }

        public class StartScreenItem {
            public String AppId { get; set; }
            public PreviewTile Tile { get; set; }
            public PreviewTileUpdater Updater { get; set; }
        }

        private async void StartLiveTilesPage_OnLoaded(Object sender, RoutedEventArgs e) {
            var packageManager = new PackageManager();
            var packages = (IEnumerable<Windows.ApplicationModel.Package>)packageManager.FindPackagesForUserWithPackageTypes("", PackageTypes.Main);

            try {
                var liveTilesManager = new LiveTilesManager();
                List<TileModel> tiles = await liveTilesManager.GetLiveTiles();

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
                        Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                        Child = item.Tile
                    };

                    tile.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 2);
                    tile.SetValue(VariableSizedWrapGrid.RowSpanProperty, 2);
                    tile.Margin = new Thickness(1.5);
                    tile.CornerRadius = new CornerRadius(4);
                    this.StartScreen.Children.Add(tile);
                });
            } catch {
                // FXIME
            }
        }
    }
}
