using System;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfoPair
    {
        public String StyleName { get; private set; }
        public TextStyleInfo TextStyleInfo { get; private set; }

        public TextStyleInfoPair(String styleName, TextStyleInfo textStyleInfo)
        {
            this.StyleName = styleName;
            this.TextStyleInfo = textStyleInfo;
        }
    }
}
