using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace NotificationsVisualizerLibrary
{
    public sealed class PreviewTileVisualElements : INotifyPropertyChanged
    {
        private Color _backgroundColor = Colors.Blue;
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { SetProperty(ref _backgroundColor, value); }
        }

        private bool _showNameOnSquare150x150Logo = false;
        public bool ShowNameOnSquare150x150Logo
        {
            get { return _showNameOnSquare150x150Logo; }
            set { SetProperty(ref _showNameOnSquare150x150Logo, value); }
        }

        private bool _showNameOnSquare310x310Logo = true;
        public bool ShowNameOnSquare310x310Logo
        {
            get { return _showNameOnSquare310x310Logo; }
            set { SetProperty(ref _showNameOnSquare310x310Logo, value); }
        }

        private bool _showNameOnWide310x150Logo = true;
        public bool ShowNameOnWide310x150Logo
        {
            get { return _showNameOnWide310x150Logo; }
            set { SetProperty(ref _showNameOnWide310x150Logo, value); }
        }

        private Uri _square150x150Logo;
        public Uri Square150x150Logo
        {
            get { return _square150x150Logo; }
            set { SetProperty(ref _square150x150Logo, value); }
        }

        private Uri _square310x310Logo;
        public Uri Square310x310Logo
        {
            get { return _square310x310Logo; }
            set { SetProperty(ref _square310x310Logo, value); }
        }

        private Uri _square44x44Logo;
        public Uri Square44x44Logo
        {
            get { return _square44x44Logo; }
            set { SetProperty(ref _square44x44Logo, value); }
        }

        private Uri _square71x71Logo;
        public Uri Square71x71Logo
        {
            get { return _square71x71Logo; }
            set { SetProperty(ref _square71x71Logo, value); }
        }

        private Uri _wide310x150Logo;
        public Uri Wide310x150Logo
        {
            get { return _wide310x150Logo; }
            set { SetProperty(ref _wide310x150Logo, value); }
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
        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;

            this.OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion




        //private void Blah()
        //{
        //    var sample = new Windows.UI.StartScreen.SecondaryTile().VisualElements;

        //    sample.BackgroundColor = Colors.Red;

        //    sample.ShowNameOnSquare150x150Logo = true;
        //    sample.ShowNameOnSquare310x310Logo = true;
        //    sample.ShowNameOnWide310x150Logo = true;

        //    sample.Square150x150Logo = new Uri("");
        //    sample.Square310x310Logo = new Uri("");
        //    sample.Square71x71Logo = new Uri("");
        //    sample.Wide310x150Logo = new Uri("");
        //}

    }
}
