using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void SetProperty<T>(ref T storage, T value, [CallerMemberName]string name = "")
        {
            if (object.Equals(storage, value))
            {
                return;
            }

            storage = value;
            NotifyPropertyChanged(name);
        }

        internal void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    internal class BindableBaseHelper
    {
        internal static void SetProperty<T>(object sender, ref T storage, T value, PropertyChangedEventHandler handler, [CallerMemberName]string name = "")
        {
            if (object.Equals(storage, value))
            {
                return;
            }

            storage = value;
            handler?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
    }
}
