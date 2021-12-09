using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using static Shell.Pages.StartPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shell.Pages {
    public sealed partial class StartLiveTilesPage : Page {

        private Double ScreenWidth;
        private Double ScreenHeight;
        private StartScrenParameters Arguments;

        public StartLiveTilesPage() {
            this.InitializeComponent();
            this.Visibility = Visibility.Collapsed;

            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            this.Arguments = (StartScrenParameters)e.Parameter;
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
                this.StartScreenScrollViewer.Margin = new Thickness(0);
                this.AllAppsBtn.Padding = new Thickness(0);
            } else {
                if (this.ScreenHeight <= 1050) {
                    ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).MaximumRowsOrColumns = 6;
                } else {
                    ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).MaximumRowsOrColumns = 8;
                }

                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Vertical;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Stretch;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Center;

                this.StartScreenScrollViewer.Padding = new Thickness(this.ScreenWidth * 0.05);
                this.StartScreenScrollViewer.Margin = new Thickness(0, 0, 0, ((this.ScreenWidth * 0.05) * -1) - 14);
                this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.075, 0, this.ScreenWidth * 0.05, this.ScreenWidth * 0.05);
            }
        }

        private void StartLiveTilesPage_OnLoaded(Object sender, RoutedEventArgs e) {
            foreach (TileModel tile in this.Arguments.LiveTilesManager.LiveTiles) {
                var inGridItem = (GridViewItem)this.LiveTiles.ItemContainerGenerator.ContainerFromItem(tile);
                if (inGridItem != null) {
                    inGridItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, tile.RowSpawn);
                    inGridItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, tile.ColumnSpawn);
                }
            }

            this.Visibility = Visibility.Visible;

            // Trigger reflow.
            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private void LiveTile_Loaded(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((Grid)sender).DataContext;
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

            // Set tile size & density.
            tile.TileSize = item.Size;
            tile.TileDensity = item.Density;

            // Update layout.
            tile.UpdateLayout();

            // Quick-exit if no tile updates are found.
            var currentTileUpdates = this.Arguments.LiveTilesManager.LiveTilesData.FindAll(update => update.AppId == item.AppId);
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
            var item = (TileModel)((Grid)sender).DataContext;
            item.Launcher();
        }

        private void LiveTileContext_Click(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((ToggleMenuFlyoutItem)sender).DataContext;
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

        private void AllAppsBtn_Tapped(Object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            if (this.Arguments == null) return;

            this.Arguments.AllAppsBtnCallback();
        }

        private void LiveTiles_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            ((GridView)sender).SelectedItem = null;
        }
    }
}
