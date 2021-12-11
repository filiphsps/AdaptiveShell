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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.System;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ActionBar : Window {
        public ActionBar() {
            this.InitializeComponent();

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = SystemParameters.VirtualScreenHeight - 50;
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e) {
            var wndHelper = new WindowInteropHelper(this);

            Int32 exStyle = (Int32)WinAPI.GetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (Int32)WinAPI.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WinAPI.SetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void BackBtn_Click(Object sender, RoutedEventArgs e) {
            this.Close();
        }

        private async void StartBtn_Click(Object sender, RoutedEventArgs e) {
            // TODO: do this properly, in other words; we should completely host the start screen.
            var tasks = Functions.GetActiveTasks();
            foreach (WindowData task in tasks) {
                if (task.title != "Start Screen")
                    continue;

                // TODO: toggle active
                var wndHelper = new WindowInteropHelper(this);

                WinAPI.SetForegroundWindow(task.hwnd);
                WinAPI.ShowWindow(task.hwnd, (Int32)WinAPI.WindowShowStyle.ShowMaximized);
                WinAPI.SetWindowPos(task.hwnd, wndHelper.Handle, 0, 15, (Int32)SystemParameters.WorkArea.Width, (Int32)SystemParameters.WorkArea.Height - (50 + 15), (Int32)WinAPI.WindowShowStyle.ShowMaximized);

                Int32 exStyle = (Int32)WinAPI.GetWindowLong(task.hwnd, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE);
                exStyle |= (Int32)WinAPI.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
                WinAPI.SetWindowLong(task.hwnd, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
                // WinAPI.SetWindowLong(task.hwnd, (Int32)WinAPI.GetWindowLongFields.GWL_STYLE, (IntPtr)WinAPI.ExtendedWindowStyles.WS_POPUP);
                return;
            }

            Boolean res = await Launcher.LaunchUriAsync(new Uri("shell:"));
            this.StartBtn_Click(sender, e);
        }
    }
}
