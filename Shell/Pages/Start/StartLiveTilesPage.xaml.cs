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
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Foundation;

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

        public ImageBrush Logo { get; set; }
        public Action Launcher { get; set; }
    }

    public sealed partial class StartLiveTilesPage : Page {
        // Only used while using the XAML designer, e.g. design-time.
        private ObservableCollection<StartScreenItem> designerTileCollection = new ObservableCollection<StartScreenItem>() {
            new StartScreenItem {
                AppId = "1",
                DisplayName = "Small",
                Size = TileSize.Small
            },
            new StartScreenItem {
                AppId = "2",
                DisplayName = "Medium",
                Size = TileSize.Medium
            },
            new StartScreenItem {
                AppId = "3",
                DisplayName = "Wide",
                Size = TileSize.Wide
            },
            new StartScreenItem {
                AppId = "4",
                DisplayName = "Large",
                Size = TileSize.Large
            }
        };

        public ObservableCollection<StartScreenItem> tileCollection = new ObservableCollection<StartScreenItem>();
        public List<TileModel> tileUpdates = new List<TileModel>();

        private Double ScreenWidth;
        private Double ScreenHeight;

        public StartLiveTilesPage() {
            this.InitializeComponent();

            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private void StartLiveTilesPage_SizeChanged(Object sender, SizeChangedEventArgs e) {
            this.ScreenWidth = Window.Current.CoreWindow.Bounds.Width;
            this.ScreenHeight = Window.Current.CoreWindow.Bounds.Height;

            if (this.LiveTiles.ItemsPanelRoot == null)
                return;
            
            if (this.ScreenWidth <= 950) {
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Horizontal;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Center;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Stretch;

                this.StartScreenScrollViewer.Padding = new Thickness(0);
            } else {
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Vertical;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Stretch;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Center;

                this.StartScreenScrollViewer.Padding = new Thickness(this.ScreenWidth * 0.05);
            }
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
                _ = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () => {
                    if (package.DisplayName.Length <= 0)
                        return;

                    if (package.IsFramework || package.IsResourcePackage || package.IsOptional || package.IsBundle)
                        return;

                    foreach (AppListEntry runtime in package.GetAppListEntries()) {
                        if (runtime.DisplayInfo.DisplayName.Length <= 0 || runtime.DisplayInfo.DisplayName == "NoUIEntryPoints-DesignMode")
                            continue;

                        var item = new StartScreenItem {
                            AppId = runtime.AppUserModelId,
                            Size = TileSize.Medium,
                            DisplayName = runtime.DisplayInfo.DisplayName,
                            Launcher = () => _ = runtime.LaunchAsync()
                        };

                        this.tileCollection.Add(item);

                        var inGridItem = (GridViewItem)this.LiveTiles.ItemContainerGenerator.ContainerFromItem(item);
                        if (inGridItem != null) {
                            inGridItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpawn);
                            inGridItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpawn);

                            try {
                                // Set logo
                                var imageStream = await runtime.DisplayInfo.GetLogo(new Windows.Foundation.Size(250, 250)).OpenReadAsync();

                                var memStream = new MemoryStream();
                                await imageStream.AsStreamForRead().CopyToAsync(memStream);
                                memStream.Position = 0;
                                var bitmap = new BitmapImage();
                                bitmap.SetSource(memStream.AsRandomAccessStream());

                                ((StartScreenItem)inGridItem.Content).Logo = new ImageBrush() {
                                    ImageSource = bitmap,
                                    Stretch = Stretch.UniformToFill
                                };
                            } catch {
                                // TODO
                            }
                        }
                    }
                });
            });

            // Trigger reflow.
            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private void LiveTile_Loaded(Object sender, RoutedEventArgs e) {
            var item = (StartScreenItem)((Grid)sender).DataContext;
            var tile = (PreviewTile)((Grid)((Grid)sender).Children[0]).Children[0];
            item.LiveTile = tile;

            // Set logo
            ((Grid)((Grid)sender).Children[0]).Background = item.Logo;

            // this shouldn't be needed
            item.DisplayName = tile.DisplayName;

            // Set tile background as transparent.
            tile.VisualElements.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            // Show name on medium tile.
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;

            // Quick-exit if no tile updates are found.
            var currentTileUpdates = this.tileUpdates.FindAll(update => update.AppId == item.AppId);
            if (currentTileUpdates.Count <= 0) {
                _ = tile.UpdateAsync();
                return;
            }

            PreviewTileUpdater tileUpdater = tile.CreateTileUpdater();

            // FIXME: Queue
            foreach (var update in currentTileUpdates) {
                tileUpdater.Update(new TileNotification(update.Payload));
            }

            PreviewBadgeUpdater badgeUpdater = tile.CreateBadgeUpdater();

            // Push updates.
            _ = tile.UpdateAsync();

            // Unset logo since we have a live tile.
            // FIXME: only do this if a live tile exists for the current size
            ((Grid)((Grid)sender).Children[0]).Background = null;
        }

        private async void LiveTile_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            // TODO
            var item = (StartScreenItem)((Grid)sender).DataContext;
            item.Launcher();
        }

        private void LiveTileContext_Click(Object sender, RoutedEventArgs e) {
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

            // FIXME: figure out why checkmark doesn't update.
            tile.UpdateLayout();

            // this shouldn't be needed
            tile.TileSize = item.Size;
            tile.DisplayName = item.DisplayName;

            var gridItem = this.LiveTiles.ItemContainerGenerator.ContainerFromItem(item);

            if (gridItem != null) {
                gridItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpawn);
                gridItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpawn);
            }

            // Push updates
            _ = tile.UpdateAsync();
        }

        private void LiveTilesLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.StartLiveTilesPage_SizeChanged(null, null);
        }
    }
}
