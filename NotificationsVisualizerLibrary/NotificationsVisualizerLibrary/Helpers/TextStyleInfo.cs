using Microsoft.UI.Xaml;
using System;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfo
    {
        public String XamlName { get; private set; }
        public Int32 Index { get; private set; }
        public TextLineBounds TextLineBounds { get; private set; }

        public TextStyleInfo(String xamlName, Int32 index, TextLineBounds textLineBounds = TextLineBounds.Full)
        {
            this.XamlName = xamlName;
            this.Index = index;
            this.TextLineBounds = textLineBounds;
        }
    }
}
