using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using Windows.Management.Deployment;
using Windows.System;
using WindowsInput;
using WindowsInput.Native;

namespace Shell.Host {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ActionBar : Window {
        public Action ToggleStart;
        public Action HideStart;
        public InputSimulator InputSimulator = new InputSimulator();

        public ActionBar() {
            this.InitializeComponent();
            this.Window_SizeChanged();

            var settings = ((Shell.Host.App)Application.Current).Settings;
            if (settings.EnableActionBar == false) {
                this.Visibility = Visibility.Collapsed;
            }
        }

        public void Window_SizeChanged() {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Height /* - Functions.ACTIONBAR_HEIGHT */; // desktopWorkingArea already accounts for ACTIONBAR_HEIGHT
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e) {
            var wndHelper = new WindowInteropHelper(this);

            // Disable focus
            WinAPI.SetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE, 
                (IntPtr)((Int32)WinAPI.GetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE) | (Int32)WinAPI.ExtendedWindowStyles.WS_EX_NOACTIVATE)
            );

            Int32 exStyle = (Int32)WinAPI.GetWindowLong(
                wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE 
            );

            exStyle |= (Int32)WinAPI.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            WinAPI.SetWindowLong(wndHelper.Handle, (Int32)WinAPI.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void ActionBarControl_ChildChanged(Object sender, EventArgs e) {
            var windowsXamlHost = sender as global::Microsoft.Toolkit.Wpf.UI.XamlHost.WindowsXamlHost;

            if (windowsXamlHost == null)
                return; // TODO: handle this.

            var control = windowsXamlHost.GetUwpInternalObject() as global::Shell.Controls.ActionBarControl;

            if (control == null)
                return; // TODO: handle this.

            // var settings = ((Shell.Host.App)Application.Current).Settings;
            control.Height = this.Height;
            control.Width = this.Width;
            control.ActionBarItemHeight = new Windows.UI.Xaml.GridLength(Functions.ACTIONBAR_HEIGHT);
            control.ActionBarItemWidth = new Windows.UI.Xaml.GridLength(Functions.ACTIONBAR_HEIGHT);

            control.OnBack += () => {
                Debug.WriteLine("Back requested!");
                // Press top bar to make sure current window is active
                // TODO: Hook into the actual winapi back function
                this.InputSimulator.Mouse.MoveMouseBy(0, (Int32)SystemParameters.WorkArea.Height * -1);
                this.InputSimulator.Mouse.LeftButtonClick();
                this.InputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.LEFT);
                this.InputSimulator.Mouse.MoveMouseBy(0, (Int32)(SystemParameters.WorkArea.Height - (Functions.ACTIONBAR_HEIGHT / 2)));
                // Restore mouse state
            };
            control.OnTaskView += () => {
                Debug.WriteLine("TaskView requested!");
                this.ToggleTaskView();
            };
            control.OnStart += () => {
                if (this.ToggleStart == null) return;

                this.ToggleStart();
            };
            control.OnSearch += () => {
                Debug.WriteLine("Search requested!");
                this.ToggleSearch();
            };
        }
        private void ToggleTaskView() {
            this.HideStart();
            this.InputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.TAB);
        }

        private void ToggleSearch() {
            this.HideStart();
            this.InputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_S);
        }
    }
}
