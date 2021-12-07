using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastSelectionBoxItem", NotificationType.Toast)]
    internal class Selection : AdaptiveChildElement
    {
        public Selection(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const string ATTR_ID = "id";
        internal const string ATTR_CONTENT = "content";

        public string Id { get; set; }

        public string Content { get; set; }

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        protected virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // id is required
            XAttribute attrId = attributes.PopAttribute(ATTR_ID);
            if (attrId == null)
            {
                result.AddError("id attribute on input element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            // type is required
            XAttribute attrContent = attributes.PopAttribute(ATTR_CONTENT);
            if (attrContent == null)
            {
                result.AddErrorButRenderAllowed("content attribute on input element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Id = attrId.Value;
            this.Content = attrContent.Value;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        public override string ToString()
        {
            return Content;
        }

        public override ObjectModelObject ConvertToObject()
        {
            var obj = base.ConvertToObject();
            obj.ConstructorValues.Add(new ObjectModelString(Id));
            obj.ConstructorValues.Add(new ObjectModelString(Content));
            return obj;
        }
    }
}
