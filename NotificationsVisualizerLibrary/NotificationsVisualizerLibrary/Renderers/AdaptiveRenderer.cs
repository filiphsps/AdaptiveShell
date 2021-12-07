using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Helpers;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Renderers
{
    internal static class AdaptiveRenderer
    {
        public static UIElement Render(AdaptiveContainer adaptiveContainer, Thickness externalMargin)
        {
            AdaptiveStackPanel answer = new AdaptiveStackPanel()
            {
                IsTopLevel = true
            };

            VerticalAlignment valignment;
            switch (adaptiveContainer.HintTextStacking)
            {
                case HintTextStacking.Bottom:
                    valignment = VerticalAlignment.Bottom;
                    break;

                case HintTextStacking.Center:
                    valignment = VerticalAlignment.Center;
                    break;

                default:
                    valignment = VerticalAlignment.Top;
                    break;
            }

            answer.VerticalAlignment = valignment;

            double topMarginOffset = externalMargin.Top;

            GenerateItems(
                children: adaptiveContainer.Children,
                container: answer.Children,
                isFirstGroup: true,
                isInsideGroup: false,
                topMarginOffset: 0,
                externalMargin: externalMargin);

            // If the first child is a group/subgroup, we set the top margin to 0
            if (answer.Children.FirstOrDefault() is AdaptiveStackPanel || answer.Children.FirstOrDefault() is AdaptiveGrid)
            {
                externalMargin.Top = 0;
            }

            answer.Margin = externalMargin;

            return answer;
        }

        /// <summary>
        /// Returns true if the child is displayed inline (for example, it would return false for background images or peek images since those aren't displayed in the stack panel)
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        private static bool IsChildInline(object child)
        {
            if (child is AdaptiveImage)
            {
                return (child as AdaptiveImage).Placement == Placement.Inline;
            }

            if (child is AdaptiveTextField)
            {
                return (child as AdaptiveTextField).Placement == TextPlacement.Inline;
            }

            return true;
        }

        private static Brush GetActionBackgroundBrush()
        {
            return new SolidColorBrush(Color.FromArgb(30, 128, 128, 128));
        }

        private static void GenerateItems(
            IEnumerable<AdaptiveChildElement> children,
            UIElementCollection container,
            bool isFirstGroup,
            bool isInsideGroup,
            double topMarginOffset,
            Thickness externalMargin)
        {
            int previousLineStyleIndex = -1;
            bool isFirst = true;
            bool needsImageMargin = false;

            foreach (var node in children.Where(i => IsChildInline(i)))
            {
                FrameworkElement uiChild = null;

                // If text
                if (node is AdaptiveTextField)
                {
                    uiChild = AdaptiveGenerator_Text.GenerateText(
                        textField: node as AdaptiveTextField,
                        isFirst: isFirst,
                        isFirstGroup: isFirstGroup,
                        previousLineStyleIndex: previousLineStyleIndex,
                        topMarginOffset: topMarginOffset,
                        needsImageMargin: needsImageMargin,
                        lineStyleIndex: out previousLineStyleIndex);
                    needsImageMargin = false;
                }

                // If image
                else if (node is AdaptiveImage)
                {
                    var imageNode = node as AdaptiveImage;

                    // An image on top of first group have 0 margin
                    // An image on top of another group should respond to topMarginOffset
                    // An image not on top of a group should have DefaultImageMargin
                    double topMargin = isFirst ? (isFirstGroup ? 0.0 : topMarginOffset) : AdaptiveConstants.DefaultImageMargin;

                    uiChild = AdaptiveGenerator_Image.GenerateImage(
                        imageNode: imageNode,
                        topMargin: topMargin,
                        isInsideGroup: isInsideGroup,
                        needsMargin: out needsImageMargin);
                    previousLineStyleIndex = -1;
                }

                // If group
                else if (node is AdaptiveGroup)
                {
                    uiChild = GenerateGroup(
                        group: node as AdaptiveGroup,
                        isFirstGroup: isFirstGroup && isFirst,
                        needsImageMargin: needsImageMargin,
                        externalMargin: externalMargin);
                    needsImageMargin = false;
                    previousLineStyleIndex = -1;
                }

                // If progress bar
                else if (node is AdaptiveProgress)
                {
                    uiChild = AdaptiveGenerator_Progress.Generate(
                        progress: node as AdaptiveProgress);
                    needsImageMargin = false;
                    previousLineStyleIndex = -1;
                }

                if (uiChild != null)
                {
                    isFirst = false;
                    container.Add(uiChild);
                }
            }
        }

        private static FrameworkElement GenerateGroup(
            AdaptiveGroup group,
            bool isFirstGroup,
            bool needsImageMargin,
            Thickness externalMargin)
        {
            if (group.Subgroups.Count == 0)
                return null;

            double topMarginOffset = 0;

            if (!isFirstGroup)
            {
                topMarginOffset = GetSubgroupsTopMarginOffset(group.Subgroups);
            }

            if (needsImageMargin)
            {
                // Ensure that top margin will be at least the image margin, even though we haven't an image
                if (topMarginOffset < AdaptiveConstants.DefaultImageMargin)
                {
                    topMarginOffset = AdaptiveConstants.DefaultImageMargin;
                }
            }

            Thickness groupMargin = new Thickness(
                0,
                isFirstGroup ? 0.0 : AdaptiveConstants.DefaultGroupTopMargin,
                0,
                0);

            if (group.Subgroups.Count == 1)
            {
                AdaptiveStackPanel sp = GenerateSubgroup(
                    subgroup: group.Subgroups[0],
                    isFirstGroup: isFirstGroup,
                    topMarginOffset: topMarginOffset,
                    externalMargin: externalMargin);

                sp.Margin = new Thickness(
                    sp.Margin.Left,
                    sp.Margin.Top + groupMargin.Top,
                    sp.Margin.Right,
                    sp.Margin.Bottom);

                if (group.ActionId != null)
                    sp.Background = GetActionBackgroundBrush();

                return sp;
            }

            else
            {
                Grid g = new AdaptiveGrid()
                {
                    Margin = groupMargin
                };

                // Get the calculated weights
                int[] weights = CalculateWeights(group.Subgroups);


                for (int i = 0; i < group.Subgroups.Count; i++)
                {
                    var subgroup = group.Subgroups[i];
                    int weight = weights[i];

                    //add column for the subgroup
                    g.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = weight == AdaptiveSubgroup.WEIGHT_AUTO ? GridLength.Auto : new GridLength(weight, GridUnitType.Star)
                    });

                    //and create the subgroup visual
                    var subgroupEl = GenerateSubgroup(subgroup, isFirstGroup, topMarginOffset, externalMargin);
                    Grid.SetColumn(subgroupEl, g.ColumnDefinitions.Count - 1); //assign it to this column

                    //set text stacking
                    switch (subgroup.HintTextStacking)
                    {
                        case HintTextStacking.Bottom:
                            subgroupEl.VerticalAlignment = VerticalAlignment.Bottom;
                            break;

                        case HintTextStacking.Center:
                            subgroupEl.VerticalAlignment = VerticalAlignment.Center;
                            break;
                    }

                    //and then add that subgroup visual to the group visual
                    g.Children.Add(subgroupEl);

                    //always add 8 pixel padding between columns (but not on the right, last, side)
                    if (i != group.Subgroups.Count - 1)
                        g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) });

                    if (subgroup.ActionId != null)
                        subgroupEl.Background = GetActionBackgroundBrush();
                }

                if (group.ActionId != null)
                    g.Background = GetActionBackgroundBrush();

                return g;
            }
        }

        private static int[] CalculateWeights(IList<AdaptiveSubgroup> subgroups)
        {
            int[] weights = new int[subgroups.Count];

            for (int i = 0; i < subgroups.Count; i++)
            {
                // If assigned, use its value
                if (subgroups[i].HintWeight != null)
                    weights[i] = subgroups[i].HintWeight.Value;

                // Otherwise, do some logic...
                else
                {
                    // If very first subgroup doesn't have hint-weight specified, it's assigned 50
                    if (i == 0)
                        weights[i] = 50;

                    // And the remaining get assigned 100 - the sum of preceeding weights
                    else
                        weights[i] = 100 - weights.Take(i).Where(w => w != AdaptiveSubgroup.WEIGHT_AUTO).Sum();
                }

                if (weights[i] != AdaptiveSubgroup.WEIGHT_AUTO)
                {
                    // Weights of 0 are assigned as 1
                    if (weights[i] == 0)
                        weights[i] = 1;

                    // Negative weights are turned into positive
                    else if (weights[i] < 0)
                        weights[i] *= -1;
                }
            }

            return weights;
        }

        /// <summary>
        /// Returns the offset for the top item
        /// </summary>
        /// <param name="subgroups"></param>
        /// <returns></returns>
        private static double GetSubgroupsTopMarginOffset(IList<AdaptiveSubgroup> subgroups)
        {
            bool hasImage = false;
            double maxOffset = 0.0;

            // Look through each subgroup that has children
            foreach (var subgroup in subgroups.Where(i => i.Children.Any()))
            {
                var firstNode = subgroup.Children.Where(i => IsChildInline(i)).First();
                double itemOffset = 0;

                if (firstNode is AdaptiveTextField)
                {
                    bool ignore;
                    TextStyleInfo textStyleInfo = AdaptiveGenerator_Text.GetStyleIndex((firstNode as AdaptiveTextField).HintStyle, out ignore);
                    int lineStyleIndex = textStyleInfo.Index;
                    itemOffset = AdaptiveGenerator_Text.DefaultTypeRamp[lineStyleIndex].TopOffset;
                }

                else if (firstNode is AdaptiveImage)
                {
                    hasImage = true;
                }

                if (itemOffset > maxOffset)
                    maxOffset = itemOffset;
            }

            if (hasImage && maxOffset < AdaptiveConstants.DefaultImageMargin)
                maxOffset = AdaptiveConstants.DefaultImageMargin;

            return maxOffset;
        }

        private static AdaptiveStackPanel GenerateSubgroup(
            AdaptiveSubgroup subgroup,
            bool isFirstGroup,
            double topMarginOffset,
            Thickness externalMargin)
        {
            AdaptiveStackPanel sp = new AdaptiveStackPanel();

            GenerateItems(
                children: subgroup.Children,
                container: sp.Children,
                isFirstGroup: isFirstGroup,
                isInsideGroup: true,
                topMarginOffset: topMarginOffset,
                externalMargin: externalMargin);

            // If we are in the first group, we apply the top margin to the subgroup
            // because some of the items might have negative margins
            if (isFirstGroup)
            {
                sp.Margin = new Thickness(0, externalMargin.Top, 0, 0);
            }

            return sp;
        }
    }
}
