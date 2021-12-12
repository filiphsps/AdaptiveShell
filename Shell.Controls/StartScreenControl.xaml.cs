using NotificationsVisualizerLibrary;
using Shell.LiveTilesAccessLibrary;
using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Shell.Controls {
    public sealed partial class StartScreenControl : UserControl {
        public ApplicationManager ApplicationManager { get; set; }
        public Action ToggleVisibility { get; set; }
        public Double ScreenWidth;
        public Double ScreenHeight;

        public StartScreenControl() {
            this.InitializeComponent();
        }

        private async void Control_OnLoaded(Object sender, RoutedEventArgs e) {
            // Set wallpaper
            var background = await Shell.PersonalizationLibrary.BackgroundImageManager.GetBackgroundImage();
            if (background != null)
                this.Root.Background = new ImageBrush() {
                    ImageSource = background,
                    Stretch = Stretch.UniformToFill
                };
        }

        public void Control_OnReady() {
            if (this.ApplicationManager == null) return;

            this.LiveTiles.ItemsSource = this.ApplicationManager.LiveTiles;
            this.Control_SizeChanged(null, null);
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            // FIXME: Split livetiles & applist into their own controls
            // to make this readable. For now let's do it like this just
            // to get it working.
            if (this.ScreenWidth <= 950) {
                // Overall layout
                this.StartScreenLayout.Height = Double.NaN;
                this.StartScreenLayout.Width = this.ScreenWidth;
                this.AppsListLayout.Height = Double.NaN;
                this.AppsListLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.AppsListLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Horizontal;


                // Live tiles layout
                if (this.LiveTiles.ItemsPanelRoot == null)
                    return;

                this.StartScreenScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                this.StartScreenScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.StartScreenScrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
                this.StartScreenScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).Orientation = Orientation.Horizontal;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).HorizontalAlignment = HorizontalAlignment.Center;
                ((VariableSizedWrapGrid)this.LiveTiles.ItemsPanelRoot).VerticalAlignment = VerticalAlignment.Stretch;

                this.LiveTiles.Padding = new Thickness(0);
                this.LiveTiles.Margin = new Thickness(0);
                // this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.05);
            } else {
                // Overall layout
                this.StartScreenLayout.Height = this.ScreenHeight;
                this.StartScreenLayout.Width = this.ScreenWidth;
                this.StartScreenLayout.Height = this.ScreenHeight;
                this.AppsListLayout.Width = this.ScreenWidth;

                this.StartScreenLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;
                this.AppsListLayout.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.StartScreenLayout.VerticalAlignment = VerticalAlignment.Stretch;

                this.Start.Orientation = Orientation.Vertical;


                // Live tiles layout
                if (this.LiveTiles.ItemsPanelRoot == null)
                    return;

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
                // this.StartScreenScrollViewer.Margin = new Thickness(0, padding, 0, (padding * -1));
                // this.AllAppsBtn.Padding = new Thickness(this.ScreenWidth * 0.05, 14, this.ScreenWidth * 0.025, this.ScreenWidth * 0.025);
            }

            // Force update
            this.Root.UpdateLayout();
            this.LiveTiles.UpdateLayout();
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

            // Should we really do this for every tile?
            this.Control_SizeChanged(null, null);
        }

        private async void LiveTile_Tapped(Object sender, TappedRoutedEventArgs e) {
            // TODO
            var item = (TileModel)((Grid)sender).DataContext;
            await item.Entry.LaunchAsync();

            if (this.ToggleVisibility != null)
                this.ToggleVisibility();
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

            // TODO
            ((Grid)gridItem.ContentTemplateRoot).Width = gridItem.ActualHeight;
            ((Grid)gridItem.ContentTemplateRoot).Height = gridItem.ActualWidth;

            // Push updates
            tile.UpdateLayout();
            await tile.UpdateAsync();

            // FIXME: figure out why checkmark doesn't update.
        }

        private void LiveTilesLayout_Loaded(Object sender, RoutedEventArgs e) {
            this.Control_SizeChanged(null, null);
        }

        private void LiveTiles_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            ((GridView)sender).SelectedItem = null;
        }

        private void ScrollViewer_ViewChanging(Object sender, ScrollViewerViewChangingEventArgs e) {
            Int32 MAX_DARK = 125;

            try {
                if (this.ScreenWidth <= 950) {
                    if (((ScrollViewer)sender).HorizontalOffset == e.NextView.HorizontalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.HorizontalOffset / ((ScrollViewer)sender).ViewportWidth) * MAX_DARK
                        ), 0, 0, 0)
                    };
                } else {
                    if (((ScrollViewer)sender).VerticalOffset == e.NextView.VerticalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.VerticalOffset / ((ScrollViewer)sender).ViewportHeight) * MAX_DARK
                        ), 0, 0, 0)
                    };
                }
            } catch { }
        }

        private void LiveTileWrapper_Loaded(Object sender, RoutedEventArgs e) {
            var item = (TileModel)((Grid)sender).DataContext;
            var gridItem = (GridViewItem)this.LiveTiles.ContainerFromItem(item);

            // FIXME: why do we have to do this manually?
            ((Grid)sender).Width = gridItem.ActualHeight;
            ((Grid)sender).Height = gridItem.ActualWidth;
        }
    }
}
