using AdaptiveShell.LiveTiles.Models.BaseElements;
using AdaptiveShell.LiveTiles.Models.ObjectModel;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models {
    internal interface ITile {
        Visual Visual { get; set; }
    }

    [ObjectModelClass("TileContent")]
    internal sealed class Tile : AdaptiveParentElement, ITile {
        [ObjectModelProperty("Visual")]
        public Visual Visual { get; set; }

        public Tile() : base(XmlTemplateParser.NotificationType.Tile) {
            this.Visual = new Visual(XmlTemplateParser.NotificationType.Tile);
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren() {
            return new AdaptiveChildElement[] {
                this.Visual
            }.Where(i => i != null);
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer() {
            return Array.Empty<String>();
        }
    }
}
