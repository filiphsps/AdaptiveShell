using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationsVisualizerLibrary.Controls
{
    internal class CallingButtonsPanel : Panel
    {
        private static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(Double), typeof(CallingButtonsPanel), new PropertyMetadata(12.0, OnDisplayPropertyChanged));

        public Double ColumnSpacing
        {
            get { return (Double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        private static void OnDisplayPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as CallingButtonsPanel).OnDisplayPropertyChanged(e);
        }

        private void OnDisplayPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        private static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(Double), typeof(CallingButtonsPanel), new PropertyMetadata(72.0, OnDisplayPropertyChanged));

        public Double ButtonSize
        {
            get { return (Double)GetValue(ButtonSizeProperty); }
            set { SetValue(ButtonSizeProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var childAvailableSize = new Size(Double.PositiveInfinity, this.ButtonSize);

            var answerButton = this.GetVisibleCallingButtons().LastOrDefault();

            if (answerButton != null)
                answerButton.IsAnswerButton = true;

            Boolean hasOtherButtons = false;

            foreach (var child in this.GetVisibleCallingButtons())
            {
                if (child == answerButton)
                {
                    child.Measure(new Size(availableSize.Width, this.ButtonSize));
                }

                else
                {
                    hasOtherButtons = true;

                    child.IsAnswerButton = false;

                    child.Measure(childAvailableSize);
                }
            }

            Double finalHeight;

            if (hasOtherButtons)
                finalHeight = this.ButtonSize * 2 + 24; // 24 px spacing between normal buttons and answer button

            else if (answerButton != null)
                finalHeight = this.ButtonSize;

            else
                finalHeight = 0;

            return new Size(
                availableSize.Width,
                finalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var answerButton = this.GetVisibleCallingButtons().LastOrDefault();

            Int32 countOfSecondaryButtons = Math.Max(this.GetVisibleCallingButtons().Count() - 1, 0);

            Double secondaryX = 0;

            if (countOfSecondaryButtons > 0)
            {
                Double secondaryButtonsTotalWidth = (countOfSecondaryButtons * this.ButtonSize) + ((countOfSecondaryButtons - 1) * this.ColumnSpacing);

                // Calculate starting point
                secondaryX = (finalSize.Width - secondaryButtonsTotalWidth) / 2;
            }

            Boolean hasOtherButtons = false;

            Int32 i = 0;
            foreach (var child in this.GetVisibleCallingButtons())
            {
                if (child == answerButton)
                {
                    child.Arrange(new Rect(
                        x: 0,
                        y: hasOtherButtons ? this.ButtonSize + 24 : 0,
                        width: finalSize.Width,
                        height: this.ButtonSize));
                }

                else
                {
                    hasOtherButtons = true;
                    

                    child.Arrange(new Rect(
                        new Point(secondaryX, 0),
                        child.DesiredSize));

                    secondaryX += this.ButtonSize + this.ColumnSpacing;

                    i++;
                }
            }

            return finalSize;
        }

        private static Boolean CountsForDisplay(UIElement el)
        {
            return el.Visibility == Visibility.Visible;
        }

        private Double GetWidthOfColumns(Size totalSize)
        {
            return (totalSize.Width - this.GetTotalColumnSpacing()) / this.GetNumberOfColumns();
        }

        private Double GetTotalColumnSpacing()
        {
            Int32 numOfCols = this.GetNumberOfColumns();

            if (numOfCols <= 1)
                return 0;

            return this.ColumnSpacing / (numOfCols - 1);
        }

        private Int32 GetNumberOfColumns()
        {
            // Subtract one since the last button is always the "answer" button
            return Math.Max(this.GetVisibleCallingButtons().Count() - 1, 0);
        }

        private IEnumerable<CallingButton> GetVisibleCallingButtons()
        {
            return this.Children.OfType<CallingButton>().Where(i => CountsForDisplay(i));
        }
    }
}
