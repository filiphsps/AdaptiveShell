using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            Functions.HideTaskBar();
            Functions.MakeNewDesktopArea();
        }

        private void Application_Exit(Object sender, ExitEventArgs e) {
            Functions.RestoreDesktopArea();
            Functions.ShowTaskBar();
        }
    }
}
