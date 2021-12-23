using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace AdaptiveShell.LiveTiles {
    public sealed class LiveTileVisualElements : INotifyPropertyChanged {
        private Color _backgroundColor = Colors.Blue;
        public Color BackgroundColor {
            get => this._backgroundColor;
            set => this.SetProperty(ref this._backgroundColor, value);
        }

        private Boolean _showNameOnSquare150x150Logo = false;
        public Boolean ShowNameOnSquare150x150Logo {
            get => this._showNameOnSquare150x150Logo;
            set => this.SetProperty(ref this._showNameOnSquare150x150Logo, value);
        }

        private Boolean _showNameOnSquare310x310Logo = true;
        public Boolean ShowNameOnSquare310x310Logo {
            get => this._showNameOnSquare310x310Logo;
            set => this.SetProperty(ref this._showNameOnSquare310x310Logo, value);
        }

        private Boolean _showNameOnWide310x150Logo = true;
        public Boolean ShowNameOnWide310x150Logo {
            get => this._showNameOnWide310x150Logo;
            set => this.SetProperty(ref this._showNameOnWide310x150Logo, value);
        }

        private Uri _square150x150Logo;
        public Uri Square150x150Logo {
            get => this._square150x150Logo;
            set => this.SetProperty(ref this._square150x150Logo, value);
        }

        private Uri _square310x310Logo;
        public Uri Square310x310Logo {
            get => this._square310x310Logo;
            set => this.SetProperty(ref this._square310x310Logo, value);
        }

        private Uri _square44x44Logo;
        public Uri Square44x44Logo {
            get => this._square44x44Logo;
            set => this.SetProperty(ref this._square44x44Logo, value);
        }

        private Uri _square71x71Logo;
        public Uri Square71x71Logo {
            get => this._square71x71Logo;
            set => this.SetProperty(ref this._square71x71Logo, value);
        }

        private Uri _wide310x150Logo;
        public Uri Wide310x150Logo {
            get => this._wide310x150Logo;
            set => this.SetProperty(ref this._wide310x150Logo, value);
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// From http://danrigby.com/2012/04/01/inotifypropertychanged-the-net-4-5-way-revisited/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private Boolean SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null) {
            if (Equals(storage, value)) {
                return false;
            }

            storage = value;

            this.OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] String propertyName = null) {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null) {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
