using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Shell.Controls {
    public sealed partial class StartScreenControl : UserControl {
        public StartScreenControl() {
            this.InitializeComponent();
        }

        private async void Control_OnLoaded(Object sender, RoutedEventArgs e) {
            // Set wallpaper
            var background = await Shell.PersonalizationLibrary.BackgroundImageManager.GetBackgroundImage();
            if (background != null)
                this.RootGrid.Background = new ImageBrush() {
                    ImageSource = background,
                    Stretch = Stretch.UniformToFill
                };
        }
    }
}
