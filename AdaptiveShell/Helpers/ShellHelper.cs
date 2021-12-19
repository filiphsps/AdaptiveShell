using flier268.Win32API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.Helpers {
    internal static class ShellHelper {
        public static Boolean HideTaskbar() {
            var taskBar = User32.FindWindow("Shell_TrayWnd", null);
            if (taskBar == null || taskBar == IntPtr.Zero) return false;
            User32.ShowWindow(taskBar, User32.SW_HIDE);

            return true;
        }

        public static Boolean ShowTaskbar() {
            var taskBar = User32.FindWindow("Shell_TrayWnd", null);
            if (taskBar == null || taskBar == IntPtr.Zero) return false;

            User32.ShowWindow(taskBar, User32.SW_SHOW);
            return true;
        }

        public static void ResizeDisplayArea() { }
        public static void RestoreDisplayArea() { }
    }
}
