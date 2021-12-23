using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.Controls {
    public sealed class AdaptiveStackPanel : Panel, IAdaptiveControl {
        /// <summary>
        /// This is updated after Measure has been called
        /// </summary>
        public Boolean DoesAllContentFit { get; private set; }

        #region IsTopLevel

        private static readonly DependencyProperty IsTopLevelProperty = DependencyProperty.Register("IsTopLevel", typeof(Boolean), typeof(AdaptiveStackPanel), new PropertyMetadata(false, OnIsTopLevelChanged));

        private static void OnIsTopLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as AdaptiveStackPanel).OnIsTopLevelChanged(e);
        }

        private void OnIsTopLevelChanged(DependencyPropertyChangedEventArgs e) {
            this.InvalidateMeasure();
        }

        public Boolean IsTopLevel {
            get { return (Boolean)this.GetValue(IsTopLevelProperty); }
            set { this.SetValue(IsTopLevelProperty, value); }
        }

        #endregion

        private Int32 _numberOfChildrenToDisplay;

        protected override Size MeasureOverride(Size availableSize) {
            this._numberOfChildrenToDisplay = 0;
            this.DoesAllContentFit = true;

            if (this.Children == null)
                return new Size();

            var remainingSize = new Size(availableSize.Width, availableSize.Height);
            Double totalHeight = 0;

            Boolean first = true;

            foreach (UIElement child in this.Children) {
                // If it's wrapping text, we give it infinite height so that we can find out how high a wrapping line of text would be if it could display everything
                if (IsWrappingText(child)) {
                    child.Measure(new Size(remainingSize.Width, Double.PositiveInfinity));
                } else {
                    // Add extra height for the bottom margin, since that shouldn't impact the final size on the bottom
                    child.Measure(new Size(remainingSize.Width, EnsureNotNegative(remainingSize.Height + GetBottomMargin(child))));
                }

                // Top level stack panel allows the first item to display, even if it can't fit
                Boolean allowDisplayRegardlessOfFit = this.IsTopLevel && first;

                // If it's an adaptive stack panel, and it couldn't fit all of its content, we won't end up displaying it (and also stop displaying future children)
                if (!allowDisplayRegardlessOfFit && child is IAdaptiveControl && !(child as IAdaptiveControl).DoesAllContentFit) {
                    this.DoesAllContentFit = false;

                    if (this.IsTopLevel)
                        break;
                }

                Double fullWrappingTextHeight = 0;

                if (IsWrappingText(child)) {
                    fullWrappingTextHeight = GetActualDesiredHeight(child);

                    // And then re-measure so that wrapping will drop at the correct point for display purposes
                    child.Measure(new Size(remainingSize.Width, EnsureNotNegative(remainingSize.Height + GetBottomMargin(child))));
                }

                // Get the actual desired height (takes into account Height and MinHeight properties, whereas DesiredHeight can be less than those values)
                Double actualDesiredHeight = GetActualDesiredHeight(child);

                // If the element can't fit (ignoring the bottom margin), we won't display it or future children
                if (actualDesiredHeight - Math.Max(GetBottomMargin(child), 0) > remainingSize.Height) {
                    this.DoesAllContentFit = false;

                    if (this.IsTopLevel && !IsImage(child))
                        break;
                }

                // Otherwise, the element fits, we'll include it
                this._numberOfChildrenToDisplay++;

                Double consumedHeight;

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

                if (!this.DoesAllContentFit)
                    break;
            }

            Double desiredWidth = this.Children.Count > 0 ? this.Children.Max(i => i.DesiredSize.Width) : 0;

            // Make sure height didn't exceed available height
            if (totalHeight > availableSize.Height)
                totalHeight = availableSize.Height;

            return new Size(desiredWidth, totalHeight);
        }

        private static Double EnsureNotNegative(Double value) {
            if (value < 0)
                return 0;

            return value;
        }

        private static Boolean IsWrappingText(UIElement el) {
            return IsText(el) && (el as TextBlock).TextWrapping != TextWrapping.NoWrap;
        }

        private static Boolean IsText(UIElement el) {
            return el is TextBlock;
        }

        private static Boolean IsImage(UIElement el) {
            return el is AdaptiveImageControl || el is CircleImage;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            if (this.Children == null)
                return finalSize;

            Double y = 0;

            //for (int i = 0; i < Children.Count; i++)
            for (Int32 i = 0; i < this._numberOfChildrenToDisplay; i++) {
                UIElement child = this.Children[i];

                Double actualDesiredHeight = GetActualDesiredHeight(child);
                Double actualDesiredHeightWithoutBottomMargin = Math.Max(actualDesiredHeight - Math.Max(GetBottomMargin(child), 0), 0);

                Double availableHeight = finalSize.Height - y;

                // If there's not enough height available, we consume the rest of the height
                if (actualDesiredHeightWithoutBottomMargin > availableHeight) {
                    child.Arrange(new Rect(0, y, finalSize.Width, availableHeight));

                    // Make sure we've consumed all the height
                    y = finalSize.Height;
                }

                // Otherwise, we have it consume however much height it said it needs (for width we give it the actual width, so that horizontal align functions)
                else {
                    child.Arrange(new Rect(0, y, finalSize.Width, actualDesiredHeight));

                    y += actualDesiredHeight;
                }
            }

            // And then hide the chidren that shouldn't be displayed
            for (Int32 i = this._numberOfChildrenToDisplay; i < this.Children.Count; i++) this.Children[i].Arrange(new Rect()); // Size 0, meaning it doesn't display

            return finalSize;
        }

        /// <summary>
        /// Returns the max between DesiredSize.Height, Height, and MinHeight
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        private static Double GetActualDesiredHeight(UIElement el) {
            Double actualDesiredHeight = el.DesiredSize.Height;

            if (el is FrameworkElement) {
                var frameworkEl = el as FrameworkElement;

                // If it has a specific height set, enforce that height
                if (!Double.IsNaN(frameworkEl.Height))
                    actualDesiredHeight = Math.Max(actualDesiredHeight, frameworkEl.Height + GetAffectOfMargin(el));

                // And if it has a min height set, enforce that min height
                if (!Double.IsNaN(frameworkEl.MinHeight))
                    actualDesiredHeight = Math.Max(actualDesiredHeight, frameworkEl.MinHeight + GetAffectOfMargin(el));
            }

            //return actualDesiredHeight + GetAffectOfMargin(el);
            return actualDesiredHeight;
        }

        private static Double GetAffectOfMargin(UIElement el) {
            if (el is FrameworkElement) {
                var frameworkEl = el as FrameworkElement;

                return frameworkEl.Margin.Top + frameworkEl.Margin.Bottom;
            }

            return 0;
        }

        private static Double GetTopMargin(UIElement el) {
            if (el is FrameworkElement) {
                var frameworkEl = el as FrameworkElement;

                return frameworkEl.Margin.Top;
            }

            return 0;
        }

        private static Double GetBottomMargin(UIElement el) {
            if (el is FrameworkElement) {
                var frameworkEl = el as FrameworkElement;

                return frameworkEl.Margin.Bottom;
            }

            return 0;
        }
    }
}
