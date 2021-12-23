using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Helpers {
    internal class TextStylesMap {
        private List<TextStyleInfoPair> _pairs = new List<TextStyleInfoPair>();

        public void Add(TextStyleInfoPair pair) {
            this._pairs.Add(pair);
        }

        public TextStyleInfo Find(String styleName) {
            return this._pairs.FirstOrDefault(i => i.StyleName.Equals(styleName))?.TextStyleInfo;
        }
    }
}
