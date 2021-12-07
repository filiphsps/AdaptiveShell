using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace NotificationsVisualizerLibrary.Helpers
{
    internal class TextStyleInfo
    {
        public string XamlName { get; private set; }
        public int Index { get; private set; }
        public TextLineBounds TextLineBounds { get; private set; }

        public TextStyleInfo(string xamlName, int index, TextLineBounds textLineBounds = TextLineBounds.Full)
        {
            XamlName = xamlName;
            Index = index;
            TextLineBounds = textLineBounds;
        }
    }
}
