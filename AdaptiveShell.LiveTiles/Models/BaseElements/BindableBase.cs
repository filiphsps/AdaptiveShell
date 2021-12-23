using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.BaseElements {
    public abstract class BindableBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetProperty<T>(ref T storage, T value, [CallerMemberName] String name = "") {
            if (Equals(storage, value)) {
                return;
            }

            storage = value;
            this.NotifyPropertyChanged(name);
        }

        internal void NotifyPropertyChanged(String name) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class BindableBaseHelper {
        internal static void SetProperty<T>(Object sender, ref T storage, T value, PropertyChangedEventHandler handler, [CallerMemberName] String name = "") {
            if (Equals(storage, value)) {
                return;
            }

            storage = value;
            handler?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
    }
}
