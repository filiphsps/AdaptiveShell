using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// A big thanks to https://github.com/pravin/deeshell
namespace Shell.Host {
    public partial class TaskBar : Form {
        public TaskBar() {
            this.InitializeComponent();
            //this.tableLayoutPanel.RowStyles[1] = new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, SystemInformation.VirtualScreen.Height - (15 + 50));
            WinAPI.SetWindowPos(this.Handle, (IntPtr)0, 0, SystemInformation.VirtualScreen.Height - 50, SystemInformation.VirtualScreen.Width, 50, 0x0040);
            // WinAPI.SetWindowLong(this.Handle, (Int32)WinAPI.WindowLongFlags.GWL_STYLE, (IntPtr)WinAPI.WindowStyles.WS_POPUPWINDOW);

            // Show StatusBar
            var status = new StatusBar();
            status.Show();
        }

        private void OnBackClick(Object sender, EventArgs e) {
            Application.Exit();
        }

        private void OnSearchClick(Object sender, EventArgs e) {
            Application.Exit();
        }

        private void OnWindowsClick(Object sender, EventArgs e) {
            var tasks = Functions.GetActiveTasks();

            foreach (WindowData task in tasks) {
                if (task.title != "Adaptive Shell")
                    continue;

                WinAPI.SetForegroundWindow(task.hwnd);
                WinAPI.ShowWindow(task.hwnd, (Int32)WinAPI.WindowShowStyle.ShowMaximized);
                // TODO: Remove window border
                //var style = new IntPtr((UInt32)WinAPI.WindowStyles.WS_POPUP);
                //WinAPI.SetWindowLong(task.hwnd, (Int32)WinAPI.WindowLongFlags.GWL_STYLE, style);
                return;
            }

            // TODO: Launch adaptive shell and rerun this handler
        }
    }
}
