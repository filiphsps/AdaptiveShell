using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastHeader", NotificationType.Toast)]
    internal class Header : AdaptiveParentElement
    {
        private const String ATTR_ID = "id";
        private const String ATTR_TITLE = "title";

        public Header(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        [ObjectModelProperty("Id")]
        public String Id { get; set; } = "";

        [ObjectModelProperty("Title")]
        public String Title { get; set; } = "";

        [ObjectModelProperty("Arguments")]
        public String Arguments { get; set; } = "";

        [ObjectModelProperty("ActivationType", ActivationType.Foreground)]
        public ActivationType ActivationType { get; set; } = ActivationType.Foreground;

        internal void Parse(ParseResult result, XElement node)
        {
            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(node, attributes, result);

            this.HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // We parse title first, and we only completely fail on title, since if id isn't provided,
            // we can at least still render something for the preview (we still display error though).
            // We'll assume that the developer will add the id before they send the toast.

            // title is required
            XAttribute attrTitle = attributes.PopAttribute(ATTR_TITLE);
            if (attrTitle == null)
            {
                result.AddErrorButRenderAllowed("title attribute on header element is required.", XmlTemplateParser.GetErrorPositionInfo(node));

                // We'll fail without a title
                throw new IncompleteElementException();
            }
            else
            {
                if (String.IsNullOrWhiteSpace(attrTitle.Value))
                {
                    result.AddWarning("title attribute in header element must contain a string. The header will be dropped.", XmlTemplateParser.GetErrorPositionInfo(attrTitle));
                    throw new IncompleteElementException();
                }
                else
                {
                    this.Title = attrTitle.Value;
                }
            }

            // id is required
            XAttribute attrId = attributes.PopAttribute(ATTR_ID);
            if (attrId == null)
            {
                result.AddErrorButRenderAllowed("id attribute on header element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
            }
            else
            {
                if (String.IsNullOrWhiteSpace(attrId.Value))
                {
                    result.AddWarning("id attribute in header element must contain a string. The header will be dropped.", XmlTemplateParser.GetErrorPositionInfo(attrId));
                    throw new IncompleteElementException();
                }
                else
                {
                    this.Id = attrId.Value;
                }
            }

            // arguments is required
            XAttribute attrArguments = attributes.PopAttribute(ATTR_ARGUMENTS);
            if (attrArguments == null)
            {
                result.AddErrorButRenderAllowed("arguments attribute on header element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
            }
            else
            {
                // Empty string in arguments is allowed
                this.Arguments = attrArguments.Value;
            }

            // activationType is optional
            ActivationType type;
            if (this.TryParseEnum(result, attributes, ATTR_ACTIVATIONTYPE, out type))
            {
                this.ActivationType = type;
            }
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[0];
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return new AdaptiveChildElement[0];
        }
    }
}
