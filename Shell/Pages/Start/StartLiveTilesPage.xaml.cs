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
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Shell.Pages {
    public sealed partial class StartLiveTilesPage : Page {

        private Double ScreenWidth;
        private Double ScreenHeight;
        private StartScrenParameters Arguments;

        public StartLiveTilesPage() {
            this.InitializeComponent();

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
                this.StartScreenScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                this.StartScreenScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.StartScreenScrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
                this.StartScreenScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Horizontal;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Center;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Stretch;

                this.LiveTiles.Padding = new Thickness(0);
                this.StartScreenScrollViewer.Margin = new Thickness(0);
                this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.05);
            } else {
                this.StartScreenScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                this.StartScreenScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.StartScreenScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                this.StartScreenScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                if (this.ScreenHeight <= 1050) {
                    ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).MaximumRowsOrColumns = 6;
                } else {
                    ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).MaximumRowsOrColumns = 8;
                }

                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Vertical;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Stretch;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Center;

                Double padding = this.ScreenWidth * 0.025;
                this.LiveTiles.Padding = new Thickness(padding, 0, padding, 0);
                this.StartScreenScrollViewer.Margin = new Thickness(0, padding, 0, (padding * -1));
                this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.05, 14, this.ScreenWidth * 0.025, this.ScreenWidth * 0.025);
            }
        }

        private void StartLiveTilesPage_OnLoaded(Object sender, RoutedEventArgs e) {
            // Trigger reflow.
            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private async void LiveTile_Loaded(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((PreviewTile)sender).DataContext;

            // Set span.
            var container = (GridViewItem)this.LiveTiles.ContainerFromItem(item);
            container.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpan);
            container.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpan);

            await item.LiveTile.UpdateAsync();
            if (item.TileData != null && item.TileData.Count >= 1) {
                PreviewTileUpdater tileUpdater = item.LiveTile.CreateTileUpdater();
                PreviewBadgeUpdater badgeUpdater = item.LiveTile.CreateBadgeUpdater();

                foreach (var data in item.TileData) {
                    // FIXME: Queue
                    tileUpdater.Update(new TileNotification(data.Payload));
                }

                // TODO: handle background based on tile data,
                ((Grid)((Grid)container.ContentTemplateRoot).Children[0]).Background = null;
            }

            // Push updates.
            item.LiveTile.UpdateLayout();
            await item.LiveTile.UpdateAsync();
        }

        private async void LiveTile_Tapped(Object sender, TappedRoutedEventArgs e) {
            // TODO
            var item = (TileModel)((Grid)sender).DataContext;
            await item.Entry.LaunchAsync();
        }

        private async void LiveTileContext_Click(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((MenuFlyoutItem)sender).DataContext;
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
            // Set span
            var gridItem = this.LiveTiles.ContainerFromItem(item);
            gridItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpan);
            gridItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpan);

            // Push updates
            tile.UpdateLayout();
            await tile.UpdateAsync();

            // FIXME: figure out why checkmark doesn't update.
        }

        private void LiveTilesLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.StartLiveTilesPage_SizeChanged(null, null);
        }

        private void AllAppsBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            if (this.Arguments == null) return;

            this.Arguments.AllAppsBtnCallback();
        }

        private void LiveTiles_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            ((GridView)sender).SelectedItem = null;
        }
    }
}
