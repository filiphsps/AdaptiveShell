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
    internal class EqualColumnsPanel : Panel
    {
        private static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(double), typeof(EqualColumnsPanel), new PropertyMetadata(0.0, OnDisplayPropertyChanged));

        public double ColumnSpacing
        {
            get { return (double)GetValue(ColumnSpacingProperty); }
            set { SetValue(ColumnSpacingProperty, value); }
        }

        private static void OnDisplayPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as EqualColumnsPanel).OnDisplayPropertyChanged(e);
        }

        private void OnDisplayPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double maxHeight = 0;

            double[] columnWidths = GetColumnWidths(availableSize);

            int i = 0;
            foreach (var child in this.Children.Where(x => CountsForDisplay(x)))
            {
                child.Measure(new Size(columnWidths[i], availableSize.Height));

                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);

                i++;
            }

            return new Size(availableSize.Width, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double[] columnWidths = GetColumnWidths(finalSize);

            int i = 0;
            double posX = 0;
            foreach (var child in this.Children.Where(x => CountsForDisplay(x)))
            {
                Size childFinalSize = new Size(columnWidths[i], finalSize.Height);

                child.Arrange(new Rect(
                    new Point(posX, 0),
                    childFinalSize));

                i++;
                posX += childFinalSize.Width + ColumnSpacing;
            }

            return finalSize;
        }

        private static bool CountsForDisplay(UIElement el)
        {
            return el.Visibility == Visibility.Visible;
        }

        private int GetNumberOfColumns()
        {
            return this.Children.Count(i => CountsForDisplay(i));
        }

        private double[] GetColumnWidths(Size totalSize)
        {
            int numOfCols = GetNumberOfColumns();

            if (numOfCols <= 0)
                return new double[0];

            if (numOfCols == 1)
                return new double[] { totalSize.Width };

            double[] colWidths = new double[numOfCols];

            double widthWithoutSpacing = totalSize.Width - ColumnSpacing * (numOfCols - 1);
            double remainingWidth = widthWithoutSpacing;

            for (int i = 0; i < colWidths.Length; i++)
            {
                // If it's the last one, we can allow it to have decimal width to stretch to the end
                if (i == colWidths.Length - 1)
                {
                    colWidths[i] = remainingWidth;
                    break;
                }

                // Take the ceiling of the remaining column width, ensuring it's an even integer
                double thisColWidth = Math.Ceiling(remainingWidth / (colWidths.Length - i));
                colWidths[i] = thisColWidth;
                remainingWidth -= thisColWidth;
            }

            return colWidths;
        }
    }
}
