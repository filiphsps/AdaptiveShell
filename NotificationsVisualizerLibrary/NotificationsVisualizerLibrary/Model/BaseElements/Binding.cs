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
    internal abstract class Binding : AdaptiveParentElement
    {
        protected static readonly string ATTR_BINDING_TEMPLATE = "template";
        protected static readonly string ATTR_BINDING_FALLBACK = "fallback";
        protected static readonly string ATTR_BINDING_LANG = "lang";
        protected static readonly string ATTR_BINDING_BASEURI = "baseUri";
        protected static readonly string ATTR_BINDING_ADDIMAGEQUERY = "addImageQuery";
        protected static readonly string ATTR_BINDING_CONTENTID = "contentId";



        private List<AdaptiveChildElement> _children = new List<AdaptiveChildElement>();
        public IReadOnlyList<AdaptiveChildElement> Children
        {
            get { return _children; }
        }


        public void Add(TextField child)
        {
            AddHelper(child);
        }

        public void Add(Image child)
        {
            AddHelper(child);
        }


        protected void AddHelper(AdaptiveChildElement child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return Children;
        }

        internal void Parse(ParseResult result, XElement node, string baseUri, bool addImageQuery)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            

            ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);

            // 0-n children
            foreach (XElement child in node.Elements())
            {
                try
                {
                    HandleChild(result, child, baseUri, addImageQuery);
                }

                catch (IncompleteElementException)
                {

                }
            }
        }
        

        protected virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            // BaseUri is optional
            {
                XAttribute attrBaseUri = attributes.PopAttribute(ATTR_BINDING_BASEURI);
                if (attrBaseUri != null)
                    baseUri = attrBaseUri.Value; // Overwrite cascaded value if it was specified
            }

            // AddImageQuery is optional
            {
                bool val;
                if (TryParse(result, attributes, ATTR_BINDING_ADDIMAGEQUERY, out val))
                    addImageQuery = val; // Overwrite cascaded value if it was specified
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { ATTR_BINDING_FALLBACK, ATTR_BINDING_LANG, ATTR_BINDING_CONTENTID };
        }

        protected abstract void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery);
    }
}
