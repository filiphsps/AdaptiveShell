﻿using Microsoft.Toolkit.Uwp.UI.Controls;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shell.Controls {
    public sealed partial class AppListLayoutControl : UserControl {
        public Double ScreenWidth { get; set; }
        public Double ScreenHeight { get; set; }

        public ObservableCollection<TileModel> ItemsSource { get; set; }
        public Action ToggleVisibility { get; set; }

        public AppListLayoutControl() {
            this.InitializeComponent();
        }

        public void Control_OnReady() {
            Debug.WriteLine("[AppListLayout] OnReady!");

            /* this.ItemsSource.CollectionChanged += (Object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
                this.AppsSource.Source = from c in this.ItemsSource group c by c.DisplayName[0].ToString().ToUpper();
            }; */

            this.AppsSource.Source = from c in this.ItemsSource group c by c.DisplayName[0].ToString().ToUpper();
            this.Control_SizeChanged(null, null);
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            if (this.ScreenWidth == 0) return;
            if (this.AppsList.ItemsPanelRoot == null) return;

            if (this.ScreenWidth <= 950) {
                this.RootScrollViewer.VerticalScrollMode = ScrollMode.Enabled;
                this.RootScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                this.RootScrollViewer.HorizontalScrollMode = ScrollMode.Disabled;
                this.RootScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;

                this.AppsList.HorizontalAlignment = HorizontalAlignment.Center;
                this.AppsList.VerticalAlignment = VerticalAlignment.Stretch;

                ((WrapPanel)this.AppsList.ItemsPanelRoot).Orientation = Orientation.Horizontal;
                ((WrapPanel)this.AppsList.ItemsPanelRoot).MaxHeight = Double.NaN;

                this.AppsList.Padding = new Thickness(0);
                this.AppsList.Margin = new Thickness(0);
            } else {
                this.RootScrollViewer.VerticalScrollMode = ScrollMode.Disabled;
                this.RootScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.RootScrollViewer.HorizontalScrollMode = ScrollMode.Enabled;
                this.RootScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                ((WrapPanel)this.AppsList.ItemsPanelRoot).Orientation = Orientation.Vertical;
                ((WrapPanel)this.AppsList.ItemsPanelRoot).MaxHeight = 412;

                this.AppsList.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.AppsList.VerticalAlignment = VerticalAlignment.Center;

                Double padding = this.ScreenWidth * 0.025;
                this.AppsList.Padding = new Thickness(padding, 0, padding, 0);
            }

            // Force update
            this.AppsList.UpdateLayout();
        }

        private void StackPanel_Loaded(Object sender, RoutedEventArgs e) {
            this.Control_SizeChanged(null, null);
        }

        private void AppsListItem_Tapped(Object sender, TappedRoutedEventArgs e) {
            // TODO

            if (this.ToggleVisibility != null)
                this.ToggleVisibility();
        }
    }
}