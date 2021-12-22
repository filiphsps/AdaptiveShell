using AdaptiveShell.LiveTiles.Models.ObjectModel;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AdaptiveShell.LiveTiles.Models.BaseElements {
    [ObjectModelClass("AdaptiveGroup")]
    internal class AdaptiveGroup : AdaptiveParentElement, IBindingChild {
        public AdaptiveGroup(XmlTemplateParser.NotificationType context) : base(context) { }
        public const String ATTR_ACTIONID = "actionId";

        [ObjectModelProperty("Children")]
        public IList<AdaptiveSubgroup> Subgroups { get; private set; } = new List<AdaptiveSubgroup>();

        public String ActionId { get; set; }

        public void Add(AdaptiveSubgroup element) {
            this.Subgroups.Add(element);
            element.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren() {
            return this.Subgroups;
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer() {
            return new String[0];
        }

        internal void Parse(ParseResult result, XElement node, String baseUri, Boolean addImageQuery) {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            this.HandleRemainingAttributes(attributes, result);

            foreach (XElement n in node.Elements()) {
                try {
                    this.HandleChild(result, n, baseUri, addImageQuery);
                } catch (IncompleteElementException) { }
            }
        }

        internal void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery) {
            if (this.Context != XmlTemplateParser.NotificationType.Tile) {
                // actionId is optional
                XAttribute attrActionId = attributes.PopAttribute(ATTR_ACTIONID);
                if (attrActionId != null)
                    this.ActionId = attrActionId.Value;
            }
        }

        protected void HandleChild(ParseResult result, XElement child, String baseUri, Boolean addImageQuery) {
            if (child.IsType("subgroup")) {
                var subgroup = new AdaptiveSubgroup(this.Context);
                subgroup.Parse(result, child, baseUri, addImageQuery);

                if (!result.IsOkForRender())
                    throw new IncompleteElementException();

                if (subgroup != null)
                    this.Add(subgroup);
            } else {
                result.AddError($@"Invalid child ""{child.Name.LocalName}"" found in a group. Groups can only contain subgroups.", GetErrorPositionInfo(child));
            }
        }
    }
}
