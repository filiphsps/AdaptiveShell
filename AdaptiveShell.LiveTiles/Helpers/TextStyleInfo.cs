using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Helpers {
    internal class TextStyleInfo {
        public String XamlName { get; private set; }
        public Int32 Index { get; private set; }
        public TextLineBounds TextLineBounds { get; private set; }

        public TextStyleInfo(String xamlName, Int32 index, TextLineBounds textLineBounds = TextLineBounds.Full) {
            this.XamlName = xamlName;
            this.Index = index;
            this.TextLineBounds = textLineBounds;
        }
    }
}
