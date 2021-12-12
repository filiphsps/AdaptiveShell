using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shell.Controls {
    public sealed partial class LiveTilesLayoutControl : UserControl {
        public Double ScreenWidth { get; set; }
        public Double ScreenHeight { get; set; }

        public ObservableCollection<TileModel> ItemsSource { get; set; }

        public LiveTilesLayoutControl() {
            this.InitializeComponent();
        }

        public void Control_OnReady() {
            Debug.WriteLine("[LiveTilesLayout] OnReady!");
            this.LiveTiles.ItemsSource = this.ItemsSource;
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            if (this.ScreenWidth <= 950) {
                this.RootScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                this.RootScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.RootScrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
                this.RootScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Horizontal;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Center;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Stretch;

                this.LiveTiles.Padding = new Thickness(0);
                this.LiveTiles.Margin = new Thickness(0);
                // this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.05);
            } else {
                this.RootScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                this.RootScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.RootScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                this.RootScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

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
                // this.StartScreenScrollViewer.Margin = new Thickness(0, padding, 0, (padding * -1));
                // this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.05, 14, this.ScreenWidth * 0.025, this.ScreenWidth * 0.025);
            }

            // Force update
            this.LiveTiles.UpdateLayout();
        }

        private async void LiveTile_Loaded(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((PreviewTile)sender).DataContext;

            Debug.WriteLine($"[LiveTilesLayout] LiveTile_Loaded {item.AppId} - {item.DisplayName}");

            // Set span.
            var container = (GridViewItem)this.LiveTiles.ContainerFromItem(item);
            container.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpan);
            container.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpan);
            this.LiveTileRoot_Loaded(container.ContentTemplateRoot, null);

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

            // Should we really do this for every tile?
            this.Control_SizeChanged(null, null);
        }

        private async void LiveTile_Tapped(Object sender, TappedRoutedEventArgs e) {
            // TODO
            var item = (TileModel)((Grid)sender).DataContext;
            await item.Entry.LaunchAsync();

            /* if (this.ToggleVisibility != null)
                this.ToggleVisibility(); */
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
            var gridItem = (GridViewItem)this.LiveTiles.ContainerFromItem(item);
            gridItem.SetValue(VariableSizedWrapGrid.RowSpanProperty, item.RowSpan);
            gridItem.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, item.ColumnSpan);
            this.LiveTileRoot_Loaded(gridItem.ContentTemplateRoot, null);

            // Push updates
            tile.UpdateLayout();
            await tile.UpdateAsync();
        }

        private void LiveTilesLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.Control_SizeChanged(null, null);
        }

        private void LiveTiles_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            ((GridView)sender).SelectedItem = null;
        }

        private void LiveTileRoot_Loaded(Object sender, RoutedEventArgs e) {
            var gridItem = (Grid)sender;
            var container = (GridViewItem)this.LiveTiles.ContainerFromItem(gridItem.DataContext);

            // why do we have to do this? aggghhhhh
            gridItem.Height = container.ActualHeight;
            gridItem.Width = container.ActualWidth;

        }
    }
}
