using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Windowing;
using AdaptiveShell.LiveTiles.Models;
using AdaptiveShell.ViewModels;
using Microsoft.UI;
using Windows.Graphics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell {
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LauncherWindow : Window {
        private Frame rootFrame;
        private Microsoft.UI.Xaml.Navigation.NavigatedEventHandler rootFrameLoadedHandler;

        public LauncherWindow() {
            this.InitializeComponent();
        }

        public void Loaded() {
            IntPtr handle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId window = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
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

            RectInt32 workArea = screen.WorkArea;
            SizeInt32 size = app.Size;

            // Center splash on screen.
            app.Move(new Windows.Graphics.PointInt32((workArea.Width - size.Width) / 2, (workArea.Height - size.Height) / 2));

            // TODO: do all setup and preloading here.

            // Navigate to Start page
            this.rootFrame = new Frame {
                Height = size.Height,
                Width = size.Width
            };

            // Wait for page load.
            this.rootFrameLoadedHandler = (Object sender, NavigationEventArgs e) => {
                var start = this.rootFrame.Content as Views.StartPage;

                // TODO: data context.
                start.DataContext = new ViewModels.StartViewModel() {
                    /* LiveTiles = new ObservableCollection<LiveTileModel> {
                        new LiveTileModel {
                            AppId = "1"
                        },
                        new LiveTileModel {
                            AppId = "2"
                        },
                        new LiveTileModel {
                            AppId = "3"
                        }
                    } */
                };

                // Fill workarea.
                if (!System.Diagnostics.Debugger.IsAttached) {
                    app.MoveAndResize(workArea);
                }

                // Listen to window resizes and update start accordingly.
                this.SizeChanged += (Object sender, WindowSizeChangedEventArgs args) => {
                    start.WindowSize = new Windows.Graphics.SizeInt32((Int32)args.Size.Width, (Int32)args.Size.Height);
                    this.rootFrame.Width = args.Size.Width;
                    this.rootFrame.Height = args.Size.Height;
                };

                // Unsubscribe from future events.
                this.rootFrame.Navigated -= this.rootFrameLoadedHandler;

                // Show frame.
                this.Content = this.rootFrame;

                // Set window.
                ((StartViewModel)start.DataContext).Window = this;
            };

            this.rootFrame.Navigated += this.rootFrameLoadedHandler;
            this.rootFrame.Navigate(typeof(Views.StartPage), workArea, new Microsoft.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }
    }
}
