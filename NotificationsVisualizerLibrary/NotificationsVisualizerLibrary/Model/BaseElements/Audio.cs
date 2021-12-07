using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastAudio", NotificationType.Toast)]
    internal class Audio : AdaptiveChildElement
    {
        public Audio(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private const string ATTR_SRC = "src";
        private const string ATTR_LOOP = "loop";
        private const string ATTR_SILENT = "silent";

        [ObjectModelProperty("Src")]
        public Uri Src { get; set; }

        [ObjectModelProperty("Loop", false)]
        public bool Loop { get; set; }

        [ObjectModelProperty("Silent", false)]
        public bool Silent { get; set; }

        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            AttributesHelper attributes = new AttributesHelper(node.Attributes());
            
            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        internal virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // src is optional
            var srcAttr = attributes.PopAttribute(ATTR_SRC);
            if (srcAttr != null)
            {
                Uri srcUri;
                if (Uri.TryCreate(srcAttr.Value, UriKind.RelativeOrAbsolute, out srcUri))
                {
                    Src = srcUri;
                }
                else
                {
                    result.AddWarning("Audio src value must be a valid Uri.", XmlTemplateParser.GetErrorPositionInfo(node));
                }
            }

            // loop is optional, must be bool
            bool boolean;
            if (TryParse(result, attributes, ATTR_LOOP, out boolean))
            {
                Loop = boolean;
            }

            if (TryParse(result, attributes, ATTR_SILENT, out boolean))
            {
                Silent = boolean;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }
    }
}
