using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TemplateVisualizerLibrary.Model.Enums;
using TemplateVisualizerLibrary.Parsers;

namespace TemplateVisualizerLibrary.Model
{
    internal class Image : AdaptiveChildElement, IBindingChild
    {
        private static readonly string ATTR_IMAGE_SRC = "src";
        private static readonly string ATTR_IMAGE_ALT = "alt";
        private static readonly string ATTR_IMAGE_ADDIMAGEQUERY = "addImageQuery";
        private static readonly string ATTR_IMAGE_HINT_CROP = "hint-crop";

        public HintCrop HintCrop { get; set; } = HintCrop.None;

        /// <summary>
        /// Required
        /// </summary>
        public string Src { get; set; }


        internal void Parse(ParseResult result, XElement node, string baseUri, bool addImageQuery)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);
        }

        internal virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            // src is required
            XAttribute attrSrc = attributes.PopAttribute(ATTR_IMAGE_SRC);
            if (attrSrc == null)
            {
                result.AddError("src attribute on image element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Src = XmlTemplateParser.ConstructUri(attrSrc.Value, baseUri, addImageQuery);



            // alt is optional, I don't use it right now either
            attributes.PopAttribute(ATTR_IMAGE_ALT);


            // hint-crop is optional
            HintCrop hintCrop;
            if (TryParseEnum(result, attributes, ATTR_IMAGE_HINT_CROP, out hintCrop))
                this.HintCrop = hintCrop;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { ATTR_IMAGE_ADDIMAGEQUERY };
        }
    }
}
