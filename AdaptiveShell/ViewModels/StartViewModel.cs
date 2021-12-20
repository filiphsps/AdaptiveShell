using AdaptiveShell.LiveTiles.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AdaptiveShell.ViewModels {
    class StartViewModel : ObservableRecipient {
        public StartViewModel() {
            this.RequestPinLiveTileCommand = new RelayCommand(this.RequestPinLiveTile);
            this.RequestUnPinLiveTileCommand = new RelayCommand(this.RequestUnPinLiveTile);
        }

        private ObservableCollection<LiveTileModel> _liveTiles;
        public ObservableCollection<LiveTileModel> LiveTiles { 
            get => this._liveTiles;
            set => this.SetProperty(ref this._liveTiles, value, true);
        }

        public ICommand RequestPinLiveTileCommand { get; }
        public ICommand RequestUnPinLiveTileCommand { get; }

        public void RequestPinLiveTile() {
            return; // TODO
        }

        public void RequestUnPinLiveTile() {
            return; // TODO
        }
    }
}
