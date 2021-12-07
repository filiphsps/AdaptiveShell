using NotificationsVisualizerLibrary.Helpers;
using NotificationsVisualizerLibrary.Model;
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

namespace NotificationsVisualizerLibrary.Controls
{
    internal class AdaptiveGenerator_Text
    {
        private static readonly SolidColorBrush TextBlockBrush = new SolidColorBrush(Colors.White);
        private const double SubtleOpacity = 0.6;
        private const OpticalMarginAlignment DefaultOpticalMarginAlignment = OpticalMarginAlignment.TrimSideBearings;
        private const TextTrimming DefaultTextTrimming = TextTrimming.CharacterEllipsis;
        private const TextWrapping DefaultTextWrapping = TextWrapping.NoWrap;

        public static StyleInfo[] DefaultTypeRamp = new StyleInfo[]
        {
            //              LineHeight minLineHeight SFirstGRP TopOffset        { Caption Body Subtitle Title SubHeader Header TitleNumeral SubheaderNumeral HeaderNumeral
            new StyleInfo(  16,           16.0,          4,         4, new double[] { 0,     1,     0,      0,     0,       0,       4,            8,              8, }), // 0 Caption
            new StyleInfo(  20,           20.0,          3,         5, new double[] { 1,     0,     0,      0,     0,       0,       5,            7,              8, }),    // 1 Body
            new StyleInfo(  24,           26.6,          0,         7, new double[] { 2,     1,     0,      0,     0,       0,       6,            8,             10, }),    // 2 Subtitle
            new StyleInfo(  30,           31.9,          0,         9, new double[] { 1,     1,     0,      0,     0,       0,       7,            9,             12, }),    // 3 Title
            new StyleInfo(  40,           45.2,          0,        12, new double[] { 1,     4,     0,      0,     0,       0,      10,           12,             15, }),    // 4 SubHeader 
            new StyleInfo(  56,           61.3,          0,        17, new double[] { 4,     5,     3,      5,     0,       0,      12,           16,             20, }),    // 5 Header
            new StyleInfo(  17.9,         17.9,          8,         0, new double[] { 3,     4,     5,      6,    10,       0,      12,           14,             16, }),    // 6 TitleNumeral
            new StyleInfo(  24.8,         24.8,          8,         0, new double[] { 6,     4,     5,      6,     3,       3,      14,           14,             16, }),    // 7 SubHeaderNumeral
            new StyleInfo(  33.3,         33.3,          8,         0, new double[] { 8,     7,     6,      8,     6,       5,      16,           16,             20, }),    // 8 HeaderNumeral
        };

        public static TextStyleInfo DefaultStyleInfo;

        private static TextStylesMap _stylesMap;

        private static void EnsureStyleTable()
        {
            if (DefaultTypeRamp.Length != AdaptiveConstants.KNOWN_TEXT_STYLES_COUNT)
                throw new Exception("DefaultTypeRamp size is incorrect");

            _stylesMap = new TextStylesMap();

            DefaultStyleInfo = new TextStyleInfo("CaptionTextBlockStyle", 0);

            _stylesMap.Add(new TextStyleInfoPair("caption", DefaultStyleInfo));
            _stylesMap.Add(new TextStyleInfoPair("body", new TextStyleInfo("BodyTextBlockStyle", 1)));
            _stylesMap.Add(new TextStyleInfoPair("base", new TextStyleInfo("BaseTextBlockStyle", 1)));
            _stylesMap.Add(new TextStyleInfoPair("subtitle", new TextStyleInfo("SubtitleTextBlockStyle", 2)));
            _stylesMap.Add(new TextStyleInfoPair("title", new TextStyleInfo("TitleTextBlockStyle", 3)));
            _stylesMap.Add(new TextStyleInfoPair("subheader", new TextStyleInfo("SubheaderTextBlockStyle", 4)));
            _stylesMap.Add(new TextStyleInfoPair("header", new TextStyleInfo("HeaderTextBlockStyle", 5)));
            _stylesMap.Add(new TextStyleInfoPair("titlenumeral", new TextStyleInfo("TitleTextBlockStyle", 6, TextLineBounds.Tight)));
            _stylesMap.Add(new TextStyleInfoPair("subheadernumeral", new TextStyleInfo("SubheaderTextBlockStyle", 7, TextLineBounds.Tight)));
            _stylesMap.Add(new TextStyleInfoPair("headernumeral", new TextStyleInfo("HeaderTextBlockStyle", 8, TextLineBounds.Tight)));
        }

        static AdaptiveGenerator_Text()
        {
            EnsureStyleTable();
        }

        private static TextStyleInfo GetStyleIndex(string styleName, out bool subtle)
        {
            const string subtleSuffix = "subtle";

            styleName = styleName.ToLower();

            subtle = styleName.EndsWith(subtleSuffix);
            if (subtle)
            {
                styleName = styleName.Substring(0, styleName.Length - subtleSuffix.Length);
            }

            TextStyleInfo textStyleInfo = _stylesMap.Find(styleName);

            // If not found
            if (textStyleInfo == null)
            {
                // Clear subtle style
                subtle = false;
                return DefaultStyleInfo;
            }

            return textStyleInfo;
        }

        public static TextStyleInfo GetStyleIndex(HintStyle style, out bool subtle)
        {
            if (style == HintStyle.Default)
            {
                style = HintStyle.Caption;
            }

            return GetStyleIndex(style.ToString(), out subtle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textField"></param>
        /// <param name="isFirst">Whether this is the first top-level item on the surface.</param>
        /// <param name="isFirstGroup">Whether this is inside a group.</param>
        /// <param name="previousLineStyleIndex"></param>
        /// <param name="topMarginOffset"></param>
        /// <param name="needsImageMargin"></param>
        /// <param name="lineStyleIndex"></param>
        /// <returns></returns>
        public static TextBlock GenerateText(
            AdaptiveTextField textField, 
            bool isFirst, 
            bool isFirstGroup, 
            int previousLineStyleIndex, 
            double topMarginOffset,
            bool needsImageMargin,
            out int lineStyleIndex)
        {
            lineStyleIndex = -1;

            TextBlock spTextBlock = new TextBlock()
            {
                //Foreground = TextBlockBrush,
                Text = textField.Text,
                TextWrapping = DefaultTextWrapping
            };

            // Get the text style info
            bool isSubtle;
            TextStyleInfo textStyleInfo = GetStyleIndex(textField.HintStyle, out isSubtle);

            // Set the XAML text style
            SetStyle(spTextBlock, textStyleInfo.XamlName);

            // If it's subtle
            if (isSubtle)
            {
                // Apply subtle opacity
                spTextBlock.Opacity = SubtleOpacity;
            }

            // Get the style index
            lineStyleIndex = textStyleInfo.Index;

            // Default top margin values for textblocks
            double topMargin;
            if (isFirst)
            {
                if (isFirstGroup)
                {
                    topMargin = DefaultTypeRamp[lineStyleIndex].FirstGroupMargin - AdaptiveConstants.DefaultExternalMargin;
                }

                else
                {
                    topMargin = topMarginOffset - DefaultTypeRamp[lineStyleIndex].TopOffset;
                }
            }

            else if (previousLineStyleIndex != -1)
            {
                topMargin = DefaultTypeRamp[previousLineStyleIndex].TopMarginValues[lineStyleIndex];
            }

            else if (needsImageMargin)
            {
                topMargin = AdaptiveConstants.DefaultImageMargin;
            }

            else
            {
                topMargin = DefaultTypeRamp[0].TopMarginValues[lineStyleIndex];
            }

            // Override if needed line height
            if (DefaultTypeRamp[lineStyleIndex].LineHeightOverride > 0.0)
            {
                spTextBlock.LineHeight = DefaultTypeRamp[lineStyleIndex].LineHeightOverride;
            }

            // Text wrapping
            if (textField.HintWrap.GetValueOrDefault(false))
            {
                spTextBlock.TextWrapping = TextWrapping.Wrap;
            }

            // Max lines
            if (textField.HintMaxLines != null)
            {
                spTextBlock.MaxLines = textField.HintMaxLines.Value;
            }

            // Align
            switch (textField.HintAlign)
            {
                case Model.Enums.HintAlign.Center:
                    spTextBlock.TextAlignment = TextAlignment.Center;
                    break;

                case Model.Enums.HintAlign.Right:
                    spTextBlock.TextAlignment = TextAlignment.Right;
                    break;

                case Model.Enums.HintAlign.Left:
                default:
                    spTextBlock.TextAlignment = TextAlignment.Left;
                    break;
            }
            spTextBlock.OpticalMarginAlignment = DefaultOpticalMarginAlignment;
            spTextBlock.TextLineBounds = textStyleInfo.TextLineBounds;
            spTextBlock.TextTrimming = DefaultTextTrimming;

            // Min lines (default of HintMinLines is 1)
            spTextBlock.MinHeight = textField.HintMinLines.GetValueOrDefault(1) * DefaultTypeRamp[lineStyleIndex].MinLineHeight;

            // Margins
            Thickness margins = new Thickness(0, topMargin, 0, 0);
            spTextBlock.Margin = margins;

            return spTextBlock;
        }

        private static void SetStyle(TextBlock textBlock, string xamlStyleName)
        {
            textBlock.Style = (Style)Application.Current.Resources[xamlStyleName];
        }
    }
}
