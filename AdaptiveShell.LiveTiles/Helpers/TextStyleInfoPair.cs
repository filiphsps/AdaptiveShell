using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Helpers {
    internal class TextStyleInfoPair {
        public String StyleName { get; private set; }
        public TextStyleInfo TextStyleInfo { get; private set; }

        public TextStyleInfoPair(String styleName, TextStyleInfo textStyleInfo) {
            this.StyleName = styleName;
            this.TextStyleInfo = textStyleInfo;
        }
    }
}
