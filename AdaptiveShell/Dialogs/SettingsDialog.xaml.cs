using AdaptiveShell.Views.SettingsPages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace AdaptiveShell.Dialogs {
    public sealed partial class SettingsDialog : ContentDialog {
        public Window Window;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FrameworkElement _rootAppElement;
        public SettingsDialog(Window window) {
            this.InitializeComponent();
            this.Window = window;

            // We need to manually set XamlRoot.
            // see: https://github.com/microsoft/microsoft-ui-xaml/issues/4990
            this.XamlRoot = this.Window.Content.XamlRoot;
            this._rootAppElement = this.Window.Content as FrameworkElement;

            this.SettingsPane.SelectedItem = this.SettingsPane.MenuItems[0];
            this.UpdateDialogLayout();
        }

        private void UpdateDialogLayout() {
            if (this.Window.Bounds.Width <= 700) {
                this.SettingsPane.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
                this.SettingsContentFrame.Width = 410;
                this.Column0.Width = new GridLength(60);
            } else {
                this.SettingsPane.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
                this.SettingsContentFrame.Width = 460;
                this.Column0.Width = new GridLength(0, GridUnitType.Auto);
            }

            if (this.Window.Bounds.Height <= 600) {
                this.ContainerGrid.Height = this.Window.Bounds.Height;
            } else {
                this.ContainerGrid.Height = 600;
            }
        }

        private void SettingsPane_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args) {
            var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            Int32 selectedItemTag = Convert.ToInt32(selectedItem.Tag);

            _ = selectedItemTag switch {
                0 => this.SettingsContentFrame.Navigate(typeof(About)),
                1 => this.SettingsContentFrame.Navigate(typeof(About)),
                _ => this.SettingsContentFrame.Navigate(typeof(About))
            };
        }

        private void ButtonClose_Click(Object sender, RoutedEventArgs e) {
            this.Hide();
        }
    }
}
