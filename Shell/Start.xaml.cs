﻿using Microsoft.UI.Xaml;

namespace Shell {
    public sealed partial class Start : Application {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public Start() {
            //this.Initialize();
            this.InitializeComponent();
            //this.Suspending += this.OnSuspending;
        }
    }
}