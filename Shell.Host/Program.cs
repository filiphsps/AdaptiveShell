using System;
using System.Collections.Generic;
using System.Text;

namespace Shell.Host {
    public class Program {
        [System.STAThreadAttribute()]
        public static void Main() {
            // TODO: Hook into low-level keyboard to capture the physical windows button being pressed.
            // See https://github.com/shanselman/babysmash/blob/master/App.xaml.cs
            Functions.HideTaskBar();
            Functions.MakeNewDesktopArea();

            try {
                using (new Shell.Start()) {
                    var app = new Shell.Host.App();
                    app.InitializeComponent();
                    app.Run();
                }
            } catch { }

            // TODO: unhook keyboard
            Functions.RestoreDesktopArea();
            Functions.ShowTaskBar();
        }
    }
}
