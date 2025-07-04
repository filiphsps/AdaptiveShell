using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("ToastBindingGeneric", NotificationType.Toast)]
    [ObjectModelClass("TileBinding", NotificationType.Tile)]
    internal partial class AdaptiveBinding : AdaptiveParentElement
    {
        protected static readonly String ATTR_BINDING_TEMPLATE = "template";
        protected static readonly String ATTR_BINDING_FALLBACK = "fallback";
        protected static readonly String ATTR_BINDING_LANG = "lang";
        protected static readonly String ATTR_BINDING_BASEURI = "baseUri";
        protected static readonly String ATTR_BINDING_ADDIMAGEQUERY = "addImageQuery";
        protected static readonly String ATTR_BINDING_CONTENTID = "contentId";


        private static readonly String ATTR_BINDING_BRANDING = "branding";
        private static readonly String ATTR_BINDING_DISPLAY_NAME = "displayName";
        private static readonly String ATTR_BINDING_HINT_OVERLAY = "hint-overlay";
        private const String ATTR_BINDING_ARGUMENTS = "arguments";
        private const String ATTR_BINDING_HINTLOCK1 = "hint-lockDetailedStatus1";
        private const String ATTR_BINDING_HINTLOCK2 = "hint-lockDetailedStatus2";
        private const String ATTR_BINDING_HINTLOCK3 = "hint-lockDetailedStatus3";

        public AdaptiveBinding(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        public Template Template { get; set; } = Template.Unsupported;

        public AdaptiveContainer Container { get; private set; }

        [ObjectModelProperty("Branding", null, NotificationType.Tile)]
        public Branding? Branding { get; set; } = null;

        [ObjectModelProperty("Language")]
        public String Language { get; set; }

        [ObjectModelProperty("BaseUri")]
        public Uri BaseUri { get; set; }

        [ObjectModelProperty("AddImageQuery")]
        public Boolean? AddImageQuery { get; set; }

        private Double? _hintOverlay = null;
        public Double? HintOverlay
        {
            get { return this._hintOverlay; }
            set
            {
                if (value != null)
                {
                    if (value.Value < 0)
                        this._hintOverlay = 0;

                    else if (value.Value > 100)
                        this._hintOverlay = 100;

                    else
                        this._hintOverlay = value;
                }

                else
                    this._hintOverlay = value;
            }
        }

        [ObjectModelProperty("DisplayName", null, NotificationType.Tile)]
        public String DisplayName { get; set; }

        [ObjectModelProperty("Arguments", null, NotificationType.Tile)]
        public String Arguments { get; set; }

        public String HintLockDetailedStatus1 { get; set; }
        public String HintLockDetailedStatus2 { get; set; }
        public String HintLockDetailedStatus3 { get; set; }

        [ObjectModelProperty("Content", null, NotificationType.Tile)]
        public IObjectModelValue ContentForTileModel
        {
            get
            {
                return new ObjectModelObject("TileBindingContentAdaptive")
                {
                    { "TextStacking", this.Container.HintTextStacking != HintTextStacking.Default ? new ObjectModelEnum("TileTextStacking", this.Container.HintTextStacking.ToString()) : null },
                    { "Children", new ObjectModelListInitialization(this.ChildrenForObjectModel.Select(i => i.ConvertToObject())) },
                    { "BackgroundImage", this.GetImage(Placement.Background)?.ConvertToObject() },
                    { "PeekImage", this.GetImage(Placement.Peek)?.ConvertToObject() }
                };
            }
        }

        [ObjectModelProperty("Children", null, NotificationType.Toast)]
        public IEnumerable<AdaptiveChildElement> ChildrenForObjectModel
        {
            get
            {
                return this.Container.Children.Where(i => (i as AdaptiveTextField)?.Placement == TextPlacement.Inline
                    || (i as AdaptiveImage)?.Placement == Placement.Inline
                    || i is AdaptiveGroup
                    || i is AdaptiveProgress);
            }
        }

        [ObjectModelProperty("HeroImage", null, NotificationType.Toast)]
        public AdaptiveImage HeroImage
        {
            get
            {
                return this.GetImage(Placement.Hero);
            }
        }

        [ObjectModelProperty("AppLogoOverride", null, NotificationType.Toast)]
        public AdaptiveImage AppLogoOverride
        {
            get
            {
                return this.GetImage(Placement.AppLogoOverride);
            }
        }

        private AdaptiveImage GetImage(Placement placement)
        {
            return this.Container.Children.OfType<AdaptiveImage>().FirstOrDefault(i => i.Placement == placement);
        }

        [ObjectModelProperty("Attribution", null, NotificationType.Toast)]
        public AdaptiveTextField Attribution
        {
            get
            {
                return this.Container.Children.OfType<AdaptiveTextField>().FirstOrDefault(i => i.Placement == TextPlacement.Attribution);
            }
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return this.Container.GetAllChildren();
        }

        internal void Parse(ParseResult result, XElement node, String baseUri, Boolean addImageQuery)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            var attributes = new AttributesHelper(node.Attributes());



            this.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            var container = new AdaptiveContainer(this.Context, this.SupportedFeatures);
            container.ContinueParsing(result, node, attributes, node.Elements(), baseUri, addImageQuery);
            this.Container = container;

            this.HandleRemainingAttributes(attributes, result);
        }
        

        protected virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery)
        {
            // Template is required
            XAttribute attrTemplate = attributes.PopAttribute(ATTR_BINDING_TEMPLATE);
            if (attrTemplate == null)
            {
                result.AddWarning("template attribute wasn't provided on binding element.", GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            // If template is unknown, stop there
            Template template;
            if (!this.TryParseEnum(attrTemplate.Value, out template))
            {
                result.AddWarning($"template attribute \"{attrTemplate.Value}\" is not supported.", GetErrorPositionInfo(attrTemplate));
                throw new IncompleteElementException();
            }

            this.Template = template;

            switch (this.Template)
            {
                case Template.TileLarge:
                case Template.TileMedium:
                case Template.TileWide:
                case Template.TileSmall:
                    this.Context = NotificationType.Tile;
                    break;

                case Template.ToastGeneric:
                    this.Context = NotificationType.Toast;
                    break;
            }

            if (this.Context != NotificationType.Toast)
            {
                // Branding is optional
                Branding branding;
                if (this.TryParseEnum(result, attributes, ATTR_BINDING_BRANDING, out branding, false)) // not case-sensitive
                    this.Branding = branding;

                // Display name is optional
                XAttribute attrDisplayName = attributes.PopAttribute(ATTR_BINDING_DISPLAY_NAME, false); // not case-sensitive
                if (attrDisplayName != null)
                    this.DisplayName = attrDisplayName.Value;

                // hint-overlay is optional
                Double hintOverlay;
                if (TryParse(result, attributes, ATTR_BINDING_HINT_OVERLAY, out hintOverlay))
                    this.HintOverlay = hintOverlay;
            }

            if (this.Context == NotificationType.Tile)
            {
                if (this.SupportedFeatures.ChaseableTiles)
                {
                    // attributes is optional
                    XAttribute attrArguments = attributes.PopAttribute(ATTR_ARGUMENTS);
                    if (attrArguments != null)
                        this.Arguments = attrArguments.Value;
                }

                if (this.Template == Template.TileWide)
                {
                    this.HintLockDetailedStatus1 = attributes.PopAttributeValue(ATTR_BINDING_HINTLOCK1);
                    this.HintLockDetailedStatus2 = attributes.PopAttributeValue(ATTR_BINDING_HINTLOCK2);
                    this.HintLockDetailedStatus3 = attributes.PopAttributeValue(ATTR_BINDING_HINTLOCK3);
                }
            }

            // BaseUri is optional
            {
                XAttribute attrBaseUri = attributes.PopAttribute(ATTR_BINDING_BASEURI);
                if (attrBaseUri != null)
                {
                    baseUri = attrBaseUri.Value; // Overwrite cascaded value if it was specified
                    Uri uri;
                    if (Uri.TryCreate(baseUri, UriKind.RelativeOrAbsolute, out uri))
                    {
                        this.BaseUri = uri;
                    }
                }
            }

            // AddImageQuery is optional
            {
                Boolean val;
                if (TryParse(result, attributes, ATTR_BINDING_ADDIMAGEQUERY, out val))
                {
                    addImageQuery = val; // Overwrite cascaded value if it was specified
                    this.AddImageQuery = val;
                }
            }
        }

        protected override Array GetSupportedEnums<TEnum>()
        {
            if (typeof(TEnum) == typeof(Template))
            {
                switch (this.Context)
                {
                    case NotificationType.Tile:
                        return new Template[] { Template.TileSmall, Template.TileMedium, Template.TileWide, Template.TileLarge };

                    case NotificationType.Toast:
                        return new Template[] { Template.ToastGeneric };
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { ATTR_BINDING_FALLBACK, ATTR_BINDING_LANG, ATTR_BINDING_CONTENTID };
        }
    }
}
