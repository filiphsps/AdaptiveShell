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
    internal class TextField : AdaptiveChildElement, IBindingChild
    {
        private static readonly string ATTR_TEXT_LANG = "lang";

        public string Text { get; set; } = "";

        internal void Parse(ParseResult result, XElement node)
        {
            // Inner text
            this.Text = node.Value;

            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        internal virtual void ParseKnownAttributes(AttributesHelper attributes, ParseResult result)
        {
            // nothing at base level right now (should support lang in future)
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { ATTR_TEXT_LANG };
        }
    }
}
