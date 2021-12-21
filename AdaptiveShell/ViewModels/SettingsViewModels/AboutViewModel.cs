using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace AdaptiveShell.ViewModels.SettingsViewModels {
    public class AboutViewModel : ObservableObject {
        public String Version {
            get {
                PackageVersion version = Package.Current.Id.Version;
                return String.Format($"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}");
            }
        }

        public String AppName => Package.Current.DisplayName;
    }
}
