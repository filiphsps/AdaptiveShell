using AdaptiveShell.LiveTiles.Models.Enums;
using AdaptiveShell.LiveTiles.Models.ObjectModel;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Popups;

namespace AdaptiveShell.LiveTiles.Models.BaseElements {
    public class AdaptiveContainer : AdaptiveParentElement {
        public AdaptiveContainer(XmlTemplateParser.NotificationType context) : base(context) { }

        private static readonly String ATTR_TEXT_STACKING = "hint-textStacking";

        private List<AdaptiveChildElement> _children = new List<AdaptiveChildElement>();
        internal IReadOnlyList<AdaptiveChildElement> Children {
            get { return this._children; }
        }

        public void SwapChildren(IEnumerable<AdaptiveChildElement> newChildren) {
            this._children.Clear();
            this._children.AddRange(newChildren);
        }

        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Default;

        /// <summary>
        /// Parses a node that has already been parsed some (for example, the Binding actually parses the node first, and then this parses some extra stuff. Binding is responsible for showing warnings about unknown unprocessed attributes after this method has parsed through what it understands).
        /// </summary>
        internal void ContinueParsing(ParseResult result, XElement node, AttributesHelper attributes, IEnumerable<XElement> children, String baseUri, Boolean addImageQuery) {
            this.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            // 0-n children
            foreach (XElement child in children) {
                try {
                    this.HandleChild(result, child, baseUri, addImageQuery);
                } catch (IncompleteElementException) {

                }
            }
        }

        protected void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery) {
            // hint-textStacking is optional
            HintTextStacking hintTextStacking;
            if (this.TryParseEnum(result, attributes, ATTR_TEXT_STACKING, out hintTextStacking))
                this.HintTextStacking = hintTextStacking;
        }

        private Boolean _hasWarnedAboutTooManyTextElements;
        private Boolean _hasWarnedAboutTooManyInlineImages;
        protected void HandleChild(ParseResult result, XElement child, String baseUri, Boolean addImageQuery) {
            switch (child.Name.LocalName.ToLower()) {
                case "text":

                    var text = new AdaptiveTextField(this.Context);
                    text.Parse(result, child, true);

                    if (!result.IsOkForRender())
                        return;

                    this.Add(text);

                    return;



                case "image":

                    var image = new AdaptiveImage(this.Context);
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                        return;

                    this.Add(image);

                    return;
            }

            result.AddWarning($"Invalid child \"{child.Name.LocalName}\" under binding element. It will be ignored.", GetErrorPositionInfo(child));
        }

        private void Add(AdaptiveTextField child) {
            this.AddHelper(child);
        }

        private void Add(AdaptiveImage child) {
            this.AddHelper(child);
        }

        private void Add(AdaptiveGroup child) {
            this.AddHelper(child);
        }

        private void AddHelper(AdaptiveChildElement child) {
            this._children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren() {
            return this.Children;
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer() {
            return new String[] { };
        }

        internal void RemoveChildAt(Int32 index) {
            this._children.RemoveAt(index);
        }
    }
}
