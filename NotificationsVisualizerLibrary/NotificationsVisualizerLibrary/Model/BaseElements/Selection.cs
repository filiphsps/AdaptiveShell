using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastSelectionBoxItem", NotificationType.Toast)]
    internal partial class Selection : AdaptiveChildElement
    {
        public Selection(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const String ATTR_ID = "id";
        internal const String ATTR_CONTENT = "content";

        public String Id { get; set; }

        public String Content { get; set; }

        internal void Parse(ParseResult result, XElement node)
        {
            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(node, attributes, result);

            this.HandleRemainingAttributes(attributes, result);
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

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { };
        }

        public override String ToString()
        {
            return this.Content;
        }

        public override ObjectModelObject ConvertToObject()
        {
            var obj = base.ConvertToObject();
            obj.ConstructorValues.Add(new ObjectModelString(this.Id));
            obj.ConstructorValues.Add(new ObjectModelString(this.Content));
            return obj;
        }
    }
}
