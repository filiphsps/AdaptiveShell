using System;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class StyleInfo
    {
        public Double LineHeightOverride;
        public Double MinLineHeight;
        public Double FirstGroupMargin;
        public Double TopOffset;
        public Double[] TopMarginValues;

        public StyleInfo(Double lineHeight, Double minLineHeight, Double firstGroupMargin, Double topOffset, Double[] topMarginValues)
        {
            this.LineHeightOverride = lineHeight;
            this.MinLineHeight = minLineHeight;
            this.FirstGroupMargin = firstGroupMargin;
            this.TopOffset = topOffset;
            this.TopMarginValues = topMarginValues;
        }
    }
}
