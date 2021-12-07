using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TemplateVisualizerLibrary.Model.Enums;
using TemplateVisualizerLibrary.Parsers;

namespace TemplateVisualizerLibrary.Model.TileElements
{
    internal class TileImage : Image
    {
        private static readonly string ATTR_IMAGE_PLACEMENT = "placement";
        private static readonly string ATTR_IMAGE_HINT_REMOVE_MARGIN = "hint-removeMargin";
        private static readonly string ATTR_IMAGE_HINT_ALIGN = "hint-align";


        public bool HintRemoveMargin { get; set; } = false;

        public HintImageAlign HintAlign { get; set; } = HintImageAlign.Stretch;

        public Placement Placement { get; set; } = Placement.Inline;

        internal override void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            base.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            // placement defaults to inline
            Placement placement;
            if (TryParseEnum(result, attributes, ATTR_IMAGE_PLACEMENT, out placement))
            {
                this.Placement = placement;
            }

            // hint-removeMargin is optional
            bool hintRemoveMargin;
            if (TryParse(result, attributes, ATTR_IMAGE_HINT_REMOVE_MARGIN, out hintRemoveMargin))
                this.HintRemoveMargin = hintRemoveMargin;

            // hint-align is optional
            HintImageAlign hintAlign;
            if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_ALIGN, out hintAlign))
                this.HintAlign = hintAlign;
        }
    }
}
