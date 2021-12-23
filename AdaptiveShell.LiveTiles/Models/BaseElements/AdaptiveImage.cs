using AdaptiveShell.LiveTiles.Models.Enums;
using AdaptiveShell.LiveTiles.Models.ObjectModel;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Popups;

namespace AdaptiveShell.LiveTiles.Models.BaseElements {
    [ObjectModelClass("AdaptiveImage")]
    internal class AdaptiveImage : AdaptiveChildElement, IBindingChild {
        public AdaptiveImage(XmlTemplateParser.NotificationType context) : base(context) { }

        private static readonly String ATTR_IMAGE_SRC = "src";
        private static readonly String ATTR_IMAGE_ALT = "alt";
        private static readonly String ATTR_IMAGE_ADDIMAGEQUERY = "addImageQuery";
        private static readonly String ATTR_IMAGE_HINT_CROP = "hint-crop";
        private static readonly String ATTR_IMAGE_PLACEMENT = "placement";
        private static readonly String ATTR_IMAGE_HINT_REMOVE_MARGIN = "hint-removeMargin";
        private static readonly String ATTR_IMAGE_HINT_ALIGN = "hint-align";
        private const String ATTR_HINT_OVERLAY = "hint-overlay";

        public String Id { get; set; }

        [ObjectModelProperty("AlternateText")]
        public String AlternateText { get; set; }

        [ObjectModelProperty("AddImageQuery")]
        public Boolean? AddImageQuery { get; set; }

        [ObjectModelProperty("HintCrop", HintCrop.Default)]
        public HintCrop HintCrop { get; set; } = HintCrop.Default;

        [ObjectModelProperty("HintRemoveMargin")]
        public Boolean? HintRemoveMargin { get; set; }

        [ObjectModelProperty("HintAlign", HintImageAlign.Default)]
        public HintImageAlign HintAlign { get; set; } = HintImageAlign.Default;

        public Enums.Placement Placement { get; set; } = Enums.Placement.Inline;

        private Double? _hintOverlay = null;
        public Double? HintOverlay {
            get { return this._hintOverlay; }
            set {
                if (value != null) {
                    if (value.Value < 0)
                        this._hintOverlay = 0;

                    else if (value.Value > 100)
                        this._hintOverlay = 100;

                    else
                        this._hintOverlay = value;
                } else
                    this._hintOverlay = value;
            }
        }

        /// <summary>
        /// Required
        /// </summary>
        [ObjectModelProperty("Source")]
        public String Src { get; set; }


        internal void Parse(ParseResult result, XElement node, String baseUri, Boolean addImageQuery) {
            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            this.HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery) {
            // AddImageQuery is optional
            {
                Boolean val;
                if (TryParse(result, attributes, ATTR_IMAGE_ADDIMAGEQUERY, out val)) {
                    addImageQuery = val; // Overwrite cascaded value if it was specified
                    this.AddImageQuery = val;
                }
            }

            // src is required
            XAttribute attrSrc = attributes.PopAttribute(ATTR_IMAGE_SRC);
            if (attrSrc == null) {
                result.AddError("src attribute on image element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Src = XmlTemplateParser.ConstructUri(attrSrc.Value, baseUri, addImageQuery);



            // alt is optional, I don't use it right now
            XAttribute altAttr = attributes.PopAttribute(ATTR_IMAGE_ALT);
            if (altAttr != null) {
                this.AlternateText = altAttr.Value;
            }

            // placement defaults to inline
            Enums.Placement placement;
            if (this.TryParseEnum(result, attributes, ATTR_IMAGE_PLACEMENT, out placement))
                this.Placement = placement;

            switch (placement) {
                case Enums.Placement.Background:
                    this.HandleHintCrop(result, attributes);
                    this.HandleHintOverlay(result, attributes);
                    break;

                case Enums.Placement.Peek:
                    this.HandleHintCrop(result, attributes);
                    this.HandleHintOverlay(result, attributes);
                    break;

                case Enums.Placement.Hero:
                    break;

                default:
                    this.HandleHintCrop(result, attributes);

                    // hint-removeMargin is optional
                    Boolean hintRemoveMargin;
                    if (TryParse(result, attributes, ATTR_IMAGE_HINT_REMOVE_MARGIN, out hintRemoveMargin))
                        this.HintRemoveMargin = hintRemoveMargin;

                    // hint-align is optional
                    HintImageAlign hintAlign;
                    if (this.TryParseEnum(result, attributes, ATTR_IMAGE_HINT_ALIGN, out hintAlign))
                        this.HintAlign = hintAlign;

                    break;
            }
        }

        private void HandleHintOverlay(ParseResult result, AttributesHelper attributes) {
            // hint-overlay is optional
            Double hintOverlay;
            if (TryParse(result, attributes, ATTR_HINT_OVERLAY, out hintOverlay))
                this.HintOverlay = hintOverlay;
        }

        private void HandleHintCrop(ParseResult result, AttributesHelper attributes) {
            HintCrop hintCrop;
            if (this.TryParseEnum(result, attributes, ATTR_IMAGE_HINT_CROP, out hintCrop))
                this.HintCrop = hintCrop;
        }

        protected override Array GetSupportedEnums<TEnum>() {
            if (typeof(TEnum) == typeof(Enums.Placement)) {
                switch (this.Context) {
                    case XmlTemplateParser.NotificationType.Tile:
                        return new Enums.Placement[] { Enums.Placement.Inline, Enums.Placement.Background, Enums.Placement.Peek };
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer() {
            return new String[] { };
        }

        public override ObjectModelObject ConvertToObject() {
            switch (this.Placement) {
                case Enums.Placement.Peek:
                    return new ObjectModelObject("TilePeekImage")
                    {
                        { "Source", this.Src },
                        { "AlternateText", this.AlternateText },
                        { "AddImageQuery", this.AddImageQuery },
                        { "HintOverlay", this.HintOverlay },
                        { "HintCrop", this.HintCrop != HintCrop.Default ? new ObjectModelEnum("TilePeekImageCrop", this.HintCrop.ToString()) : null }
                    };

                case Enums.Placement.Background:
                    return new ObjectModelObject("TileBackgroundImage")
                    {
                        { "Source", this.Src },
                        { "AlternateText", this.AlternateText },
                        { "AddImageQuery", this.AddImageQuery },
                        { "HintOverlay", this.HintOverlay },
                        { "HintCrop", this.HintCrop != HintCrop.Default ? new ObjectModelEnum("TileBackgroundImageCrop", this.HintCrop.ToString()) : null }
                    };

                default:
                    return base.ConvertToObject();
            }
        }
    }
}
