using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shell.Host {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Make new Working Area
            Functions.HideTaskBar();
            Functions.MakeNewDesktopArea();

            Application.Run(new TaskBar() {
                ShowInTaskbar = false,
                ShowIcon = false,
                Visible = false
            });

            // Restore Working Area Size
            Functions.RestoreDesktopArea();
            Functions.ShowTaskBar();
        }
    }
}
