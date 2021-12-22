using AdaptiveShell.Dialogs;
using AdaptiveShell.LiveTiles.Models;
using AdaptiveShell.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Foundation.Metadata;

namespace AdaptiveShell.ViewModels {
    class StartViewModel : ObservableRecipient {
        public Window Window { get; set; }

        //public UIElement Window { get; set; }
        public StartViewModel() {
            this.RequestPinLiveTileCommand = new RelayCommand(this.RequestPinLiveTile);
            this.RequestUnPinLiveTileCommand = new RelayCommand(this.RequestUnPinLiveTile);
            this.RequestSettingsDialogCommand = new RelayCommand(this.RequestSettingsDialog);
        }

        private ObservableCollection<LiveTileModel> _liveTiles;
        public ObservableCollection<LiveTileModel> LiveTiles { 
            get => this._liveTiles;
            set => this.SetProperty(ref this._liveTiles, value, true);
        }

        public ICommand RequestPinLiveTileCommand { get; }
        public ICommand RequestUnPinLiveTileCommand { get; }
        public ICommand RequestSettingsDialogCommand { get; }

        public void RequestPinLiveTile() {
            return; // TODO
        }

        public void RequestUnPinLiveTile() {
            return; // TODO
        }

        public async void RequestSettingsDialog() {
            var settingsDialog = new SettingsDialog(this.Window);
            _ = await settingsDialog.ShowAsync();
        }
    }
}
