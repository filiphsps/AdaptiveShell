using Microsoft.UI.Xaml.Controls;
using AdaptiveShell.LiveTiles.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Microsoft.UI.Xaml;

namespace AdaptiveShell.UserControls {
    public sealed partial class LiveTilesLayoutControl : UserControl {
        public ObservableCollection<LiveTileModel> ItemsSource {
            get => (ObservableCollection<LiveTileModel>)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource",
            typeof(ObservableCollection<LiveTileModel>), typeof(LiveTilesLayoutControl), null);

        public Boolean IsLoading => this.ItemsSource is not {Count: > 0};

        public LiveTilesLayoutControl() {
            this.InitializeComponent();
        }
    }
}
