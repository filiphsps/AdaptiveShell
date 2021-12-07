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
    public sealed class AdaptiveStackPanel : Panel, IAdaptiveControl
    {
        /// <summary>
        /// This is updated after Measure has been called
        /// </summary>
        public bool DoesAllContentFit { get; private set; }

        #region IsTopLevel

        private static readonly DependencyProperty IsTopLevelProperty = DependencyProperty.Register("IsTopLevel", typeof(bool), typeof(AdaptiveStackPanel), new PropertyMetadata(false, OnIsTopLevelChanged));

        private static void OnIsTopLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as AdaptiveStackPanel).OnIsTopLevelChanged(e);
        }

        private void OnIsTopLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            base.InvalidateMeasure();
        }

        public bool IsTopLevel
        {
            get { return (bool)GetValue(IsTopLevelProperty); }
            set { SetValue(IsTopLevelProperty, value); }
        }

        #endregion

        private int _numberOfChildrenToDisplay;

        protected override Size MeasureOverride(Size availableSize)
        {
            _numberOfChildrenToDisplay = 0;
            DoesAllContentFit = true;

            if (Children == null)
                return new Size();

            Size remainingSize = new Size(availableSize.Width, availableSize.Height);
            double totalHeight = 0;

            bool first = true;
            
            foreach (var child in Children)
            {
                // If it's wrapping text, we give it infinite height so that we can find out how high a wrapping line of text would be if it could display everything
                if (IsWrappingText(child))
                {
                    child.Measure(new Size(remainingSize.Width, double.PositiveInfinity));
                }

                else
                {
                    // Add extra height for the bottom margin, since that shouldn't impact the final size on the bottom
                    child.Measure(new Size(remainingSize.Width, EnsureNotNegative(remainingSize.Height + GetBottomMargin(child))));
                }
                
                // Top level stack panel allows the first item to display, even if it can't fit
                bool allowDisplayRegardlessOfFit = IsTopLevel && first;
                
                // If it's an adaptive stack panel, and it couldn't fit all of its content, we won't end up displaying it (and also stop displaying future children)
                if (!allowDisplayRegardlessOfFit && child is IAdaptiveControl && !(child as IAdaptiveControl).DoesAllContentFit)
                {
                    DoesAllContentFit = false;

                    if (IsTopLevel)
                        break;
                }

                double fullWrappingTextHeight = 0;

                if (IsWrappingText(child))
                {
                    fullWrappingTextHeight = GetActualDesiredHeight(child);

                    // And then re-measure so that wrapping will drop at the correct point for display purposes
                    child.Measure(new Size(remainingSize.Width, EnsureNotNegative(remainingSize.Height + GetBottomMargin(child))));
                }

                // Get the actual desired height (takes into account Height and MinHeight properties, whereas DesiredHeight can be less than those values)
                double actualDesiredHeight = GetActualDesiredHeight(child);

                // If the element can't fit (ignoring the bottom margin), we won't display it or future children
                if (actualDesiredHeight - Math.Max(GetBottomMargin(child), 0) > remainingSize.Height)
                {
                    DoesAllContentFit = false;

                    if (IsTopLevel && !IsImage(child))
                        break;
                }

                // Otherwise, the element fits, we'll include it
                _numberOfChildrenToDisplay++;

                double consumedHeight;

                // Consumed height for wrapping is the entire height of all the text if it could fit
                if (IsWrappingText(child))
                    consumedHeight = fullWrappingTextHeight;
                else
                    consumedHeight = actualDesiredHeight;
                
                // Increase the total height
                totalHeight += actualDesiredHeight;

                // Decrease the remaining size height by however much the current child wants to consume (the result could be negative since we ignore the bottom margin previously)
                remainingSize.Height = Math.Max(remainingSize.Height - consumedHeight, 0);

                first = false;

                if (!DoesAllContentFit)
                    break;
            }

            double desiredWidth = Children.Count > 0 ? Children.Max(i => i.DesiredSize.Width) : 0;

            // Make sure height didn't exceed available height
            if (totalHeight > availableSize.Height)
                totalHeight = availableSize.Height;

            return new Size(desiredWidth, totalHeight);
        }

        private static double EnsureNotNegative(double value)
        {
            if (value < 0)
                return 0;

            return value;
        }

        private static bool IsWrappingText(UIElement el)
        {
            return IsText(el) && (el as TextBlock).TextWrapping != TextWrapping.NoWrap;
        }

        private static bool IsText(UIElement el)
        {
            return el is TextBlock;
        }

        private static bool IsImage(UIElement el)
        {
            return el is AdaptiveImageControl || el is CircleImage;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children == null)
                return finalSize;

            double y = 0;
            
            //for (int i = 0; i < Children.Count; i++)
            for (int i = 0; i < _numberOfChildrenToDisplay; i++)
            {
                var child = Children[i];

                double actualDesiredHeight = GetActualDesiredHeight(child);
                double actualDesiredHeightWithoutBottomMargin = Math.Max(actualDesiredHeight - Math.Max(GetBottomMargin(child), 0), 0);

                double availableHeight = finalSize.Height - y;

                // If there's not enough height available, we consume the rest of the height
                if (actualDesiredHeightWithoutBottomMargin > availableHeight)
                {
                    child.Arrange(new Rect(0, y, finalSize.Width, availableHeight));

                    // Make sure we've consumed all the height
                    y = finalSize.Height;
                }

                // Otherwise, we have it consume however much height it said it needs (for width we give it the actual width, so that horizontal align functions)
                else
                {
                    child.Arrange(new Rect(0, y, finalSize.Width, actualDesiredHeight));

                    y += actualDesiredHeight;
                }
            }

            // And then hide the chidren that shouldn't be displayed
            for (int i = _numberOfChildrenToDisplay; i < Children.Count; i++)
                Children[i].Arrange(new Rect()); // Size 0, meaning it doesn't display

            return finalSize;
        }

        /// <summary>
        /// Returns the max between DesiredSize.Height, Height, and MinHeight
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        private static double GetActualDesiredHeight(UIElement el)
        {
            double actualDesiredHeight = el.DesiredSize.Height;

            if (el is FrameworkElement)
            {
                FrameworkElement frameworkEl = el as FrameworkElement;

                // If it has a specific height set, enforce that height
                if (!double.IsNaN(frameworkEl.Height))
                    actualDesiredHeight = Math.Max(actualDesiredHeight, frameworkEl.Height + GetAffectOfMargin(el));

                // And if it has a min height set, enforce that min height
                if (!double.IsNaN(frameworkEl.MinHeight))
                    actualDesiredHeight = Math.Max(actualDesiredHeight, frameworkEl.MinHeight + GetAffectOfMargin(el));
            }

            //return actualDesiredHeight + GetAffectOfMargin(el);
            return actualDesiredHeight;
        }

        private static double GetAffectOfMargin(UIElement el)
        {
            if (el is FrameworkElement)
            {
                FrameworkElement frameworkEl = el as FrameworkElement;

                return frameworkEl.Margin.Top + frameworkEl.Margin.Bottom;
            }

            return 0;
        }

        private static double GetTopMargin(UIElement el)
        {
            if (el is FrameworkElement)
            {
                FrameworkElement frameworkEl = el as FrameworkElement;

                return frameworkEl.Margin.Top;
            }

            return 0;
        }

        private static double GetBottomMargin(UIElement el)
        {
            if (el is FrameworkElement)
            {
                FrameworkElement frameworkEl = el as FrameworkElement;

                return frameworkEl.Margin.Bottom;
            }

            return 0;
        }
    }
}
