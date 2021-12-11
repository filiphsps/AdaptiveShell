using System;
using System.Collections.Generic;
using System.Text;

namespace Shell.Host {
    public class Program {
        [System.STAThreadAttribute()]
        public static void Main() {
            try {
                using (new Shell.Start()) {
                    var app = new Shell.Host.App();
                    app.InitializeComponent();
                    app.Run();
                }
            } catch { }
        }
    }
}
