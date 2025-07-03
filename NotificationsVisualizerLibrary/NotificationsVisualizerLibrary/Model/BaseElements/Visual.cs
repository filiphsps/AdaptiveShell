using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("ToastVisual", NotificationType.Toast)]
    [ObjectModelClass("TileVisual", NotificationType.Tile)]
    internal class Visual : AdaptiveParentElement
    {
        public Visual(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private static readonly String ATTR_VISUAL_VERSION = "version";
        private static readonly String ATTR_VISUAL_LANG = "lang";
        private static readonly String ATTR_VISUAL_BASEURI = "baseUri";
        private static readonly String ATTR_VISUAL_ADDIMAGEQUERY = "addImageQuery";
        private static readonly String ATTR_VISUAL_CONTENTID = "contentId";

        private static readonly String ATTR_VISUAL_BRANDING = "branding";
        private static readonly String ATTR_VISUAL_DISPLAY_NAME = "displayName";

        private const String ATTR_VISUAL_ARGUMENTS = "arguments";

        public Int32 Version { get; set; } = 3;

        [ObjectModelProperty("Branding", null, NotificationType.Tile)]
        public Branding? Branding { get; set; } = null;

        [ObjectModelProperty("DisplayName", null, NotificationType.Tile)]
        public String DisplayName { get; set; } = null;

        [ObjectModelProperty("Arguments", null, NotificationType.Tile)]
        public String Arguments { get; set; } = null;

        [ObjectModelProperty("Language")]
        public String Language { get; set; }

        [ObjectModelProperty("BaseUri")]
        public Uri BaseUri { get; set; }

        [ObjectModelProperty("AddImageQuery")]
        public Boolean? AddImageQuery { get; set; }

        private List<AdaptiveBinding> _bindings = new List<AdaptiveBinding>();
        public IReadOnlyList<AdaptiveBinding> Bindings { get { return this._bindings; } }

        [ObjectModelProperty("BindingGeneric", null, NotificationType.Toast)]
        public AdaptiveBinding ToastBindingGeneric
        {
            get { return this.GetBinding(Template.ToastGeneric); }
        }

        [ObjectModelProperty("TileSmall", null, NotificationType.Tile)]
        public AdaptiveBinding TileSmall
        {
            get { return this.GetBinding(Template.TileSmall); }
        }

        [ObjectModelProperty("TileMedium", null, NotificationType.Tile)]
        public AdaptiveBinding TileMedium
        {
            get { return this.GetBinding(Template.TileMedium); }
        }

        [ObjectModelProperty("TileWide", null, NotificationType.Tile)]
        public AdaptiveBinding TileWide
        {
            get { return this.GetBinding(Template.TileWide); }
        }

        [ObjectModelProperty("TileLarge", null, NotificationType.Tile)]
        public AdaptiveBinding TileLarge
        {
            get { return this.GetBinding(Template.TileLarge); }
        }

        private AdaptiveBinding GetBinding(Template template)
        {
            return this.Bindings.FirstOrDefault(i => i.Template == template);
        }

        [ObjectModelProperty("LockDetailedStatus1", null, NotificationType.Tile)]
        public String LockDetailedStatus1
        {
            get
            {
                var wide = this.TileWide;
                if (wide != null && wide.HintLockDetailedStatus1 != null)
                {
                    return wide.HintLockDetailedStatus1;
                }
                return wide?.GetAllChildren().OfType<AdaptiveTextField>().FirstOrDefault(i => Object.Equals(i.Id, "1"))?.Text;
            }
        }

        [ObjectModelProperty("LockDetailedStatus2", null, NotificationType.Tile)]
        public String LockDetailedStatus2
        {
            get
            {
                var wide = this.TileWide;
                if (wide != null && wide.HintLockDetailedStatus2 != null)
                {
                    return wide.HintLockDetailedStatus2;
                }
                return wide?.GetAllChildren().OfType<AdaptiveTextField>().FirstOrDefault(i => Object.Equals(i.Id, "2"))?.Text;
            }
        }

        [ObjectModelProperty("LockDetailedStatus3", null, NotificationType.Tile)]
        public String LockDetailedStatus3
        {
            get
            {
                var wide = this.TileWide;
                if (wide != null && wide.HintLockDetailedStatus3 != null)
                {
                    return wide.HintLockDetailedStatus3;
                }
                return wide?.GetAllChildren().OfType<AdaptiveTextField>().FirstOrDefault(i => Object.Equals(i.Id, "3"))?.Text;
            }
        }

        private void Add(AdaptiveBinding element)
        {
            this._bindings.Add(element);
            element.Parent = this;
        }


        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { ATTR_VISUAL_LANG, ATTR_VISUAL_CONTENTID };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            var attributes = new AttributesHelper(node.Attributes());

            // BaseUri is optional
            String baseUri = null;
            {
                XAttribute attrBaseUri = attributes.PopAttribute(ATTR_VISUAL_BASEURI);
                if (attrBaseUri != null)
                {
                    baseUri = attrBaseUri.Value;
                    Uri uri;
                    if (Uri.TryCreate(baseUri, UriKind.RelativeOrAbsolute, out uri))
                    {
                        this.BaseUri = uri;
                    }
                }
            }

            // AddImageQuery is optional
            Boolean addImageQuery;
            if (TryParse(result, attributes, ATTR_VISUAL_ADDIMAGEQUERY, out addImageQuery))
            {
                this.AddImageQuery = addImageQuery;
            }
            else
            {
                addImageQuery = false; // Defaults to false if not specified
            }

            if (this.Context == NotificationType.Tile)
            {
                if (this.SupportedFeatures.ChaseableTiles)
                {
                    XAttribute attrArguments = attributes.PopAttribute(ATTR_VISUAL_ARGUMENTS);
                    if (attrArguments != null)
                        this.Arguments = attrArguments.Value;
                }
            }

            this.ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            this.HandleRemainingAttributes(attributes, result);

            foreach (XElement n in node.Elements())
            {
                try
                {
                    this.HandleChild(result, n, baseUri, addImageQuery);
                }

                catch (IncompleteElementException) { }
            }
        }

        internal virtual void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery)
        {
            // version is optional
            Int32 version;
            if (TryParse(result, attributes, ATTR_VISUAL_VERSION, out version))
                this.Version = version;

            if (this.Context != NotificationType.Toast)
            {
                // Branding is optional
                Branding branding;
                if (this.TryParseEnum(result, attributes, ATTR_VISUAL_BRANDING, out branding, false)) // not case-sensitive
                    this.Branding = branding;

                // DisplayName is optional
                XAttribute attrDisplayName = attributes.PopAttribute(ATTR_VISUAL_DISPLAY_NAME, false); // not case-sensitive
                if (attrDisplayName != null)
                    this.DisplayName = attrDisplayName.Value;
            }
        }


        protected void HandleChild(ParseResult result, XElement child, String baseUri, Boolean addImageQuery)
        {
            if (child.IsType("binding"))
            {
                var binding = new AdaptiveBinding(this.Context, this.SupportedFeatures);
                binding.Parse(result, child, baseUri, addImageQuery);

                if (!result.IsOkForRender())
                    throw new IncompleteElementException();

                this.Add(binding);
            }

            else
                result.AddWarning($"Invalid child {child.Name.LocalName} under visual element.", GetErrorPositionInfo(child));
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return this.Bindings;
        }

        //public override ObjectModelObject ConvertToObject()
        //{
        //    switch (Context)
        //    {
        //        case NotificationType.Toast:
        //            return new ObjectModelObject("ToastVisual")
        //            {
        //                //{ "Language", new ObjectModelString() }
        //                //{ "BaseUri", new ObjectModelUri() }
        //                //{ "AddImageQuery" }
        //                { "BindingGeneric", Bindings.FirstOrDefault(i => i.Template == Template.ToastGeneric)?.ConvertToObject() }
        //            };

        //        default:
        //            return null;
        //    }
        //}
    }
}
