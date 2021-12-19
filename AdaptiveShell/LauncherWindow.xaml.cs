using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using flier268.Win32API;
using Microsoft.UI.Windowing;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell {
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LauncherWindow : Window {
        private Frame rootFrame;
        private Microsoft.UI.Xaml.Navigation.NavigatedEventHandler rootFrameLoadedGandler;

        public LauncherWindow() {
            this.InitializeComponent();
        }

        public void Loaded() {
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var window = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var screen = DisplayArea.GetFromWindowId(window, DisplayAreaFallback.Primary);
            var app = AppWindow.GetFromWindowId(window);

            var presenter = app.Presenter as OverlappedPresenter;

            // Normal window during debug session.
            if (!System.Diagnostics.Debugger.IsAttached) {
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsAlwaysOnTop = true;
                presenter.IsMinimizable = false;
                presenter.IsMaximizable = false;
                presenter.IsResizable = false;

                app.IsShownInSwitchers = false;
            }

            app.Title = "Start Screen";

            var workArea = screen.WorkArea;
            var size = app.Size;

            // Center splash on screen.
            app.Move(new Windows.Graphics.PointInt32((workArea.Width - size.Width) / 2, (workArea.Height - size.Height) / 2));

            // TODO: do all setup and preloading here.

            // Navigate to Start page
            this.rootFrame = new Frame {
                Height = workArea.Height,
                Width = workArea.Width
            };

            // Wait for page load.
            rootFrameLoadedGandler = (Object sender, NavigationEventArgs e) => {
                var start = rootFrame.Content as Views.StartPage;

                // Fill workarea.
                app.MoveAndResize(workArea);

                // Listen to window resizes and update start accordingly.
                this.SizeChanged += (Object sender, WindowSizeChangedEventArgs args) => {
                    start.WindowSize = new Windows.Graphics.SizeInt32((Int32)args.Size.Width, (Int32)args.Size.Height);
                };

                // Unsubscribe from future events.
                this.rootFrame.Navigated -= rootFrameLoadedGandler;

                // Show frame.
                this.Content = rootFrame;
            };

            this.rootFrame.Navigated += rootFrameLoadedGandler;
            this.rootFrame.Navigate(typeof(Views.StartPage), workArea, new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
