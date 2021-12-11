using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window {
        private ActionBar ActionBar;
        private StatusBar StatusBar;

        public HostWindow() {
            this.InitializeComponent();

            this.StatusBar = new StatusBar();
            StatusBar.Show();

            this.ActionBar = new ActionBar();
            ActionBar.Show();
            ActionBar.Closed += (Object? sender, EventArgs e) => {
                this.StatusBar.Close();
                this.Close();
            };
        }

        private void Window_Deactivated(Object sender, EventArgs e) {
            var window = (Window)sender;
            window.Topmost = true;
        }
    }
}
