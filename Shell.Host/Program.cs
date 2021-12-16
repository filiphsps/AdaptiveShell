using System;
using System.Collections.Generic;
using System.Text;

namespace Shell.Host {
    public class Program {
        [System.STAThreadAttribute()]
        public static void Main() {
            // TODO: Hook into low-level keyboard to capture the physical windows button being pressed.
            // See https://github.com/shanselman/babysmash/blob/master/App.xaml.cs

            // TODO: Setup global exception handler to restore the original state.
            // TODO: listen for applications startup and maximize windows.

            var settings = Functions.LoadSettings();

            try {
                using (new Shell.Start()) {
                    var app = new Shell.Host.App() {
                        Settings = settings
                    };

                    app.InitializeComponent();

                    Functions.HideTaskBar();
                    Functions.MakeNewDesktopArea();

                    Int32 res = app.Run();
                    settings = app.Settings;
                }
            } catch { }

            Functions.SaveSettings(settings);

            // TODO: unhook keyboard
            Functions.RestoreDesktopArea();
            Functions.ShowTaskBar();
        }
    }
}
