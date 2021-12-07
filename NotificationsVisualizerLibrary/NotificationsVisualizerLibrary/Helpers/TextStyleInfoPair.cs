using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfoPair
    {
        public string StyleName { get; private set; }
        public TextStyleInfo TextStyleInfo { get; private set; }

        public TextStyleInfoPair(string styleName, TextStyleInfo textStyleInfo)
        {
            StyleName = styleName;
            TextStyleInfo = textStyleInfo;
        }
    }
}
