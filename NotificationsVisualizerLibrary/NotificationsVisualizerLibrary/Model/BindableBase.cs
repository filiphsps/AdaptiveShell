using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetProperty<T>(ref T storage, T value, [CallerMemberName] String name = "")
        {
            if (Object.Equals(storage, value))
            {
                return;
            }

            storage = value;
            this.NotifyPropertyChanged(name);
        }

        internal void NotifyPropertyChanged(String name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    internal class BindableBaseHelper
    {
        internal static void SetProperty<T>(Object sender, ref T storage, T value, PropertyChangedEventHandler handler, [CallerMemberName] String name = "")
        {
            if (Object.Equals(storage, value))
            {
                return;
            }

            storage = value;
            handler?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
    }
}
