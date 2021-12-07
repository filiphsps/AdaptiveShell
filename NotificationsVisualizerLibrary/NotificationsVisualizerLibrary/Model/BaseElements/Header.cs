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
        private const string ATTR_ID = "id";
        private const string ATTR_TITLE = "title";

        public Header(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        [ObjectModelProperty("Id")]
        public string Id { get; set; } = "";

        [ObjectModelProperty("Title")]
        public string Title { get; set; } = "";

        [ObjectModelProperty("Arguments")]
        public string Arguments { get; set; } = "";

        [ObjectModelProperty("ActivationType", ActivationType.Foreground)]
        public ActivationType ActivationType { get; set; } = ActivationType.Foreground;

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
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
                if (string.IsNullOrWhiteSpace(attrTitle.Value))
                {
                    result.AddWarning("title attribute in header element must contain a string. The header will be dropped.", XmlTemplateParser.GetErrorPositionInfo(attrTitle));
                    throw new IncompleteElementException();
                }
                else
                {
                    Title = attrTitle.Value;
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
                if (string.IsNullOrWhiteSpace(attrId.Value))
                {
                    result.AddWarning("id attribute in header element must contain a string. The header will be dropped.", XmlTemplateParser.GetErrorPositionInfo(attrId));
                    throw new IncompleteElementException();
                }
                else
                {
                    Id = attrId.Value;
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
                Arguments = attrArguments.Value;
            }

            // activationType is optional
            ActivationType type;
            if (TryParseEnum(result, attributes, ATTR_ACTIVATIONTYPE, out type))
            {
                ActivationType = type;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[0];
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return new AdaptiveChildElement[0];
        }
    }
}
