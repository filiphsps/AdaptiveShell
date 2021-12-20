using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace AdaptiveShell.Views {
    public sealed partial class StartPage : Page {
        public Windows.Graphics.SizeInt32 WindowSize;

        public StartPage() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);

            var size = (Windows.Graphics.RectInt32)e.Parameter;
            this.WindowSize = new Windows.Graphics.SizeInt32(size.Width, size.Height);
        }

        private void RootScroll_ViewChanging(Object sender, ScrollViewerViewChangingEventArgs e) {
            Int32 MAX_DARK = 175;

            try {
                if (this.WindowSize.Width <= 950) {
                    if (((ScrollViewer)sender).HorizontalOffset == e.NextView.HorizontalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.HorizontalOffset / ((ScrollViewer)sender).ViewportWidth) * MAX_DARK
                        ), 0, 0, 0)
                    };
                } else {
                    if (((ScrollViewer)sender).VerticalOffset == e.NextView.VerticalOffset) return;

                    this.RootScroll.Background = new SolidColorBrush() {
                        Color = Windows.UI.Color.FromArgb(Convert.ToByte(
                            (e.NextView.VerticalOffset / ((ScrollViewer)sender).ViewportHeight) * MAX_DARK
                        ), 0, 0, 0)
                    };
                }
            } catch { }
        }
    }
}
