using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
        private static readonly DependencyProperty ColumnSpacingProperty = DependencyProperty.Register("ColumnSpacing", typeof(Double), typeof(EqualColumnsPanel), new PropertyMetadata(0.0, OnDisplayPropertyChanged));

        public Double ColumnSpacing
        {
            get { return (Double)this.GetValue(ColumnSpacingProperty); }
            set { this.SetValue(ColumnSpacingProperty, value); }
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
            Double maxHeight = 0;

            Double[] columnWidths = this.GetColumnWidths(availableSize);

            Int32 i = 0;
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
            Double[] columnWidths = this.GetColumnWidths(finalSize);

            Int32 i = 0;
            Double posX = 0;
            foreach (var child in this.Children.Where(x => CountsForDisplay(x)))
            {
                var childFinalSize = new Size(columnWidths[i], finalSize.Height);

                child.Arrange(new Rect(
                    new Point(posX, 0),
                    childFinalSize));

                i++;
                posX += childFinalSize.Width + this.ColumnSpacing;
            }

            return finalSize;
        }

        private static Boolean CountsForDisplay(UIElement el)
        {
            return el.Visibility == Visibility.Visible;
        }

        private Int32 GetNumberOfColumns()
        {
            return this.Children.Count(i => CountsForDisplay(i));
        }

        private Double[] GetColumnWidths(Size totalSize)
        {
            Int32 numOfCols = this.GetNumberOfColumns();

            if (numOfCols <= 0)
                return new Double[0];

            if (numOfCols == 1)
                return new Double[] { totalSize.Width };

            Double[] colWidths = new Double[numOfCols];

            Double widthWithoutSpacing = totalSize.Width - this.ColumnSpacing * (numOfCols - 1);
            Double remainingWidth = widthWithoutSpacing;

            for (Int32 i = 0; i < colWidths.Length; i++)
            {
                // If it's the last one, we can allow it to have decimal width to stretch to the end
                if (i == colWidths.Length - 1)
                {
                    colWidths[i] = remainingWidth;
                    break;
                }

                // Take the ceiling of the remaining column width, ensuring it's an even integer
                Double thisColWidth = Math.Ceiling(remainingWidth / (colWidths.Length - i));
                colWidths[i] = thisColWidth;
                remainingWidth -= thisColWidth;
            }

            return colWidths;
        }
    }
}
