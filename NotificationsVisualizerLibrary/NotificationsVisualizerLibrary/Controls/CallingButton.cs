using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Renderers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Controls
{
    internal class CallingButton : Button
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

            _textBlock = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                FontSize = 12
            };
            Grid.SetRow(_textBlock, 2);

            _image = new Image()
            {
                Stretch = Stretch.Uniform
            };

            _grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(24) },
                    new RowDefinition() { Height = new GridLength(3) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },

                Children =
                {
                    _image,
                    _textBlock
                }
            };

            this.Content = _grid;
        }

        public string Text
        {
            set
            {
                _textBlock.Text = value;
            }
        }

        public string ImageUri
        {
            set
            {
                _image.Source = ImageHelper.GetBitmap(value);
            }
        }

        public bool IsAnswerButton
        {
            set
            {
                if (value)
                {
                    Background = new SolidColorBrush((Color)Application.Current.Resources["SystemColorHighlightColor"]);
                    this.Width = double.NaN;
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
