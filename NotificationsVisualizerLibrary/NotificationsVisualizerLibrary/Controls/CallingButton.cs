using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NotificationsVisualizerLibrary.Renderers;
using Windows.UI;

namespace NotificationsVisualizerLibrary.Controls
{
    internal partial class CallingButton : Button
    {
        private Grid _grid;
        private TextBlock _textBlock;
        private Image _image;

        public CallingButton()
        {
            this.Width = 72;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.Height = 72;
            this.Background = new SolidColorBrush();

            this._textBlock = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12
            };
            Grid.SetRow(this._textBlock, 2);

            this._image = new Image()
            {
                Stretch = Stretch.Uniform
            };

            this._grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(24) },
                    new RowDefinition() { Height = new GridLength(3) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },

                Children =
                {
                    this._image,
                    this._textBlock
                }
            };

            this.Content = this._grid;
        }

        public String Text
        {
            set
            {
                this._textBlock.Text = value;
            }
        }

        public String ImageUri
        {
            set
            {
                this._image.Source = ImageHelper.GetBitmap(value);
            }
        }

        public Boolean IsAnswerButton
        {
            set
            {
                if (value)
                {
                    this.Background = new SolidColorBrush((Color)Application.Current.Resources["SystemColorHighlightColor"]);
                    this.Width = Double.NaN;
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                }

                else
                {
                    this.Background = new SolidColorBrush();
                    this.Width = 72;
                    this.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
        }
    }
}
