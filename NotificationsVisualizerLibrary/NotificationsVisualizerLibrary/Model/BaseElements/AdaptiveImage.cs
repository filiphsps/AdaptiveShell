using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveImage")]
    internal class AdaptiveImage : AdaptiveChildElement, IBindingChild
    {
        public AdaptiveImage(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private static readonly string ATTR_IMAGE_SRC = "src";
        private static readonly string ATTR_IMAGE_ALT = "alt";
        private static readonly string ATTR_IMAGE_ADDIMAGEQUERY = "addImageQuery";
        private static readonly string ATTR_IMAGE_HINT_CROP = "hint-crop";
        private static readonly string ATTR_IMAGE_PLACEMENT = "placement";
        private static readonly string ATTR_IMAGE_HINT_REMOVE_MARGIN = "hint-removeMargin";
        private static readonly string ATTR_IMAGE_HINT_ALIGN = "hint-align";
        private const string ATTR_HINT_OVERLAY = "hint-overlay";

        public string Id { get; set; }

        [ObjectModelProperty("AlternateText")]
        public string AlternateText { get; set; }

        [ObjectModelProperty("AddImageQuery")]
        public bool? AddImageQuery { get; set; }

        [ObjectModelProperty("HintCrop", HintCrop.Default)]
        public HintCrop HintCrop { get; set; } = HintCrop.Default;

        [ObjectModelProperty("HintRemoveMargin")]
        public bool? HintRemoveMargin { get; set; }

        [ObjectModelProperty("HintAlign", HintImageAlign.Default)]
        public HintImageAlign HintAlign { get; set; } = HintImageAlign.Default;

        public Placement Placement { get; set; } = Placement.Inline;

        private double? _hintOverlay = null;
        public double? HintOverlay
        {
            get { return _hintOverlay; }
            set
            {
                if (value != null)
                {
                    if (value.Value < 0)
                        _hintOverlay = 0;

                    else if (value.Value > 100)
                        _hintOverlay = 100;

                    else
                        _hintOverlay = value;
                }

                else
                    _hintOverlay = value;
            }
        }

        /// <summary>
        /// Required
        /// </summary>
        [ObjectModelProperty("Source")]
        public string Src { get; set; }


        internal void Parse(ParseResult result, XElement node, string baseUri, bool addImageQuery)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            // AddImageQuery is optional
            {
                bool val;
                if (TryParse(result, attributes, ATTR_IMAGE_ADDIMAGEQUERY, out val))
                {
                    addImageQuery = val; // Overwrite cascaded value if it was specified
                    AddImageQuery = val;
                }
            }

            // src is required
            XAttribute attrSrc = attributes.PopAttribute(ATTR_IMAGE_SRC);
            if (attrSrc == null)
            {
                result.AddError("src attribute on image element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Src = XmlTemplateParser.ConstructUri(attrSrc.Value, baseUri, addImageQuery);



            // alt is optional, I don't use it right now
            var altAttr = attributes.PopAttribute(ATTR_IMAGE_ALT);
            if (altAttr != null)
            {
                AlternateText = altAttr.Value;
            }

            // placement defaults to inline
            Placement placement;
            if (TryParseEnum(result, attributes, ATTR_IMAGE_PLACEMENT, out placement))
                this.Placement = placement;

            switch (placement)
            {
                case Placement.Background:

                    if (SupportedFeatures.CropCircleOnBackgroundImage)
                        HandleHintCrop(result, attributes);

                    if (SupportedFeatures.OverlayForBothBackgroundAndPeek)
                        HandleHintOverlay(result, attributes);

                    break;

                case Placement.Peek:

                    if (SupportedFeatures.CropCircleOnPeekImage)
                        HandleHintCrop(result, attributes);

                    if (SupportedFeatures.OverlayForBothBackgroundAndPeek)
                        HandleHintOverlay(result, attributes);

                    break;

                case Placement.Hero:
                    break;

                default:
                    HandleHintCrop(result, attributes);
                    
                    // These only apply to tiles, and only to inline images
                    if (Context != NotificationType.Toast
                        || SupportedFeatures.AdaptiveToasts
                        )
                    {
                        // hint-removeMargin is optional
                        bool hintRemoveMargin;
                        if (TryParse(result, attributes, ATTR_IMAGE_HINT_REMOVE_MARGIN, out hintRemoveMargin))
                            this.HintRemoveMargin = hintRemoveMargin;

                        // hint-align is optional
                        HintImageAlign hintAlign;
                        if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_ALIGN, out hintAlign))
                            this.HintAlign = hintAlign;
                    }

                    break;
            }
        }

        private void HandleHintOverlay(ParseResult result, AttributesHelper attributes)
        {
            // hint-overlay is optional
            double hintOverlay;
            if (TryParse(result, attributes, ATTR_HINT_OVERLAY, out hintOverlay))
                this.HintOverlay = hintOverlay;
        }

        private void HandleHintCrop(ParseResult result, AttributesHelper attributes)
        {
            HintCrop hintCrop;
            if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_CROP, out hintCrop))
                this.HintCrop = hintCrop;
        }

        protected override Array GetSupportedEnums<TEnum>()
        {
            if (typeof(TEnum) == typeof(Placement))
            {
                switch (Context)
                {
                    case NotificationType.Toast:

                        if (SupportedFeatures.RS1_Style_Toasts)
                            return new Placement[] { Placement.Inline, Placement.AppLogoOverride, Placement.Hero };

                        return new Placement[] { Placement.Inline, Placement.AppLogoOverride };

                    case NotificationType.Tile:
                        return new Placement[] { Placement.Inline, Placement.Background, Placement.Peek };
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        public override ObjectModelObject ConvertToObject()
        {
            switch (Placement)
            {
                case Placement.Hero:
                    return new ObjectModelObject("ToastGenericHeroImage")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery }
                    };

                case Placement.AppLogoOverride:
                    return new ObjectModelObject("ToastGenericAppLogo")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery },
                        { "HintCrop", HintCrop != HintCrop.Default ? new ObjectModelEnum("ToastGenericAppLogoCrop", HintCrop.ToString()) : null }
                    };

                case Placement.Peek:
                    return new ObjectModelObject("TilePeekImage")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery },
                        { "HintOverlay", HintOverlay },
                        { "HintCrop", HintCrop != HintCrop.Default ? new ObjectModelEnum("TilePeekImageCrop", HintCrop.ToString()) : null }
                    };

                case Placement.Background:
                    return new ObjectModelObject("TileBackgroundImage")
                    {
                        { "Source", Src },
                        { "AlternateText", AlternateText },
                        { "AddImageQuery", AddImageQuery },
                        { "HintOverlay", HintOverlay },
                        { "HintCrop", HintCrop != HintCrop.Default ? new ObjectModelEnum("TileBackgroundImageCrop", HintCrop.ToString()) : null }
                    };

                default:
                    return base.ConvertToObject();
            }
        }
    }
}
