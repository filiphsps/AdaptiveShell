using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal interface ITile
    {
        Visual Visual { get; set; }

        FeatureSet SupportedFeatures { get; }
    }

    [ObjectModelClass("TileContent")]
    internal sealed class Tile : AdaptiveParentElement, ITile
    {
        [ObjectModelProperty("Visual")]
        public Visual Visual { get; set; }

        public Tile(FeatureSet supportedFeatures) : base(NotificationType.Tile, supportedFeatures)
        {
            Visual = new Visual(NotificationType.Tile, SupportedFeatures);
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return new AdaptiveChildElement[]
            {
                Visual
            }.Where(i => i != null);
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[0];
        }
    }
}
