using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveSubgroup")]
    internal class AdaptiveSubgroup : AdaptiveParentElement
    {
        public AdaptiveSubgroup(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private static readonly String ATTR_SUBGROUP_HINT_WEIGHT = "hint-weight";
        private static readonly String ATTR_SUBGROUP_HINT_TEXT_STACKING = "hint-textStacking";
        public const String ATTR_ACTIONID = "actionId";

        internal const Int32 WEIGHT_AUTO = Int32.MinValue;

        private Int32? _hintWeight;
        /// <summary>
        /// Auto is WEIGHT_AUTO
        /// </summary>
        [ObjectModelProperty("HintWeight")]
        public Int32? HintWeight
        {
            get { return this._hintWeight; }
            set
            {
                if (value == null)
                    this._hintWeight = null;

                //else if (value.Value < 0)
                //    _hintWeight = 0;

                //else if (value.Value > 100)
                //    _hintWeight = 100;

                else
                    this._hintWeight = value;
            }
        }

        public String ActionId { get; set; }

        [ObjectModelProperty("Children")]
        public IList<AdaptiveChildElement> Children { get; private set; } = new List<AdaptiveChildElement>();

        [ObjectModelProperty("HintTextStacking", HintTextStacking.Default)]
        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Default;

        public void Add(AdaptiveTextField child)
        {
            this.Children.Add(child);
            child.Parent = this;
        }

        public void Add(AdaptiveImage child)
        {
            this.Children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return this.Children;
        }

        internal override AdaptiveChildElement GetNextElementBelowThisOne()
        {
            return this.Parent.GetNextElementBelowThisOne();
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[0];
        }

        internal void Parse(ParseResult result, XElement node, String baseUri, Boolean addImageQuery)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();
            

            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            this.HandleRemainingAttributes(attributes, result);

            // Children of subgroup
            foreach (XElement n in node.Elements())
            {
                try
                {
                    this.HandleChild(result, n, baseUri, addImageQuery);
                }

                catch (IncompleteElementException) { }
            }
        }

        protected void HandleChild(ParseResult result, XElement child, String baseUri, Boolean addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    var text = new AdaptiveTextField(this.Context, this.SupportedFeatures);
                    text.Parse(result, child, false);

                    if (!result.IsOkForRender())
                        throw new IncompleteElementException();

                    if (text != null)
                        this.Add(text);

                    break;



                case "image":

                    var image = new AdaptiveImage(this.Context, this.SupportedFeatures);
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                        throw new IncompleteElementException();

                    if (image != null)
                    {
                        if (image.Placement != Placement.Inline)
                        {
                            result.AddErrorButRenderAllowed("You cannot place a non-inline image element inside a subgroup.", GetErrorPositionInfo(child));
                        }

                        this.Add(image);
                    }

                    break;



                default:
                    result.AddWarning($"Invalid child \"{child.Name.LocalName}\" under subgroup element. It will be ignored.", GetErrorPositionInfo(child));
                    break;
            }
        }

        internal virtual void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery)
        {
            // hint-weight
            this.ParseHintWeight(attributes, result);

            // hint-textStacking is optional
            HintTextStacking hintTextStacking;
            if (this.TryParseEnum(result, attributes, ATTR_SUBGROUP_HINT_TEXT_STACKING, out hintTextStacking))
                this.HintTextStacking = hintTextStacking;

            if (this.Context != NotificationType.Tile && this.Context != NotificationType.Toast)
            {
                // actionId is optional
                var attrActionId = attributes.PopAttribute(ATTR_ACTIONID);
                if (attrActionId != null)
                    this.ActionId = attrActionId.Value;
            }
        }

        private void ParseHintWeight(AttributesHelper attributes, ParseResult result)
        {
            Int32 hintWeight;
            if (TryParse(result, attributes, ATTR_SUBGROUP_HINT_WEIGHT, out hintWeight))
                this.HintWeight = hintWeight;
        }
    }
}