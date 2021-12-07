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

        private static readonly string ATTR_SUBGROUP_HINT_WEIGHT = "hint-weight";
        private static readonly string ATTR_SUBGROUP_HINT_TEXT_STACKING = "hint-textStacking";
        public const string ATTR_ACTIONID = "actionId";

        internal const int WEIGHT_AUTO = int.MinValue;

        private int? _hintWeight;
        /// <summary>
        /// Auto is WEIGHT_AUTO
        /// </summary>
        [ObjectModelProperty("HintWeight")]
        public int? HintWeight
        {
            get { return _hintWeight; }
            set
            {
                if (value == null)
                    _hintWeight = null;

                //else if (value.Value < 0)
                //    _hintWeight = 0;

                //else if (value.Value > 100)
                //    _hintWeight = 100;

                else
                    _hintWeight = value;
            }
        }

        public string ActionId { get; set; }

        [ObjectModelProperty("Children")]
        public IList<AdaptiveChildElement> Children { get; private set; } = new List<AdaptiveChildElement>();

        [ObjectModelProperty("HintTextStacking", HintTextStacking.Default)]
        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Default;

        public void Add(AdaptiveTextField child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public void Add(AdaptiveImage child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return Children;
        }

        internal override AdaptiveChildElement GetNextElementBelowThisOne()
        {
            return Parent.GetNextElementBelowThisOne();
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[0];
        }

        internal void Parse(ParseResult result, XElement node, string baseUri, bool addImageQuery)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();
            

            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);

            // Children of subgroup
            foreach (XElement n in node.Elements())
            {
                try
                {
                    HandleChild(result, n, baseUri, addImageQuery);
                }

                catch (IncompleteElementException) { }
            }
        }

        protected void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    AdaptiveTextField text = new AdaptiveTextField(Context, SupportedFeatures);
                    text.Parse(result, child, false);

                    if (!result.IsOkForRender())
                        throw new IncompleteElementException();

                    if (text != null)
                        this.Add(text);

                    break;



                case "image":

                    AdaptiveImage image = new AdaptiveImage(Context, SupportedFeatures);
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

        internal virtual void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            // hint-weight
            ParseHintWeight(attributes, result);

            // hint-textStacking is optional
            HintTextStacking hintTextStacking;
            if (TryParseEnum(result, attributes, ATTR_SUBGROUP_HINT_TEXT_STACKING, out hintTextStacking))
                this.HintTextStacking = hintTextStacking;

            if (Context != NotificationType.Tile && Context != NotificationType.Toast)
            {
                // actionId is optional
                var attrActionId = attributes.PopAttribute(ATTR_ACTIONID);
                if (attrActionId != null)
                    this.ActionId = attrActionId.Value;
            }
        }

        private void ParseHintWeight(AttributesHelper attributes, ParseResult result)
        {
            int hintWeight;
            if (TryParse(result, attributes, ATTR_SUBGROUP_HINT_WEIGHT, out hintWeight))
                this.HintWeight = hintWeight;
        }
    }
}