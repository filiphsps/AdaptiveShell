using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveGroup")]
    internal class AdaptiveGroup : AdaptiveParentElement, IBindingChild
    {
        public AdaptiveGroup(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }
        public const string ATTR_ACTIONID = "actionId";

        [ObjectModelProperty("Children")]
        public IList<AdaptiveSubgroup> Subgroups { get; private set; } = new List<AdaptiveSubgroup>();

        public string ActionId { get; set; }

        public void Add(AdaptiveSubgroup element)
        {
            Subgroups.Add(element);
            element.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return Subgroups;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[0];
        }

        internal void Parse(ParseResult result, XElement node, string baseUri, bool addImageQuery)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(attributes, result, baseUri, addImageQuery);

            HandleRemainingAttributes(attributes, result);
            
            foreach (XElement n in node.Elements())
            {
                try
                {
                    HandleChild(result, n, baseUri, addImageQuery);
                }

                catch (IncompleteElementException) { }
            }
        }

        internal void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            if (Context != NotificationType.Tile && Context != NotificationType.Toast)
            {
                // actionId is optional
                var attrActionId = attributes.PopAttribute(ATTR_ACTIONID);
                if (attrActionId != null)
                    this.ActionId = attrActionId.Value;
            }
        }

        protected void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            if (child.IsType("subgroup"))
            {
                AdaptiveSubgroup subgroup = new AdaptiveSubgroup(Context, SupportedFeatures);
                subgroup.Parse(result, child, baseUri, addImageQuery);

                if (!result.IsOkForRender())
                    throw new IncompleteElementException();

                if (subgroup != null)
                    this.Add(subgroup);
            }

            else
            {
                result.AddError($@"Invalid child ""{child.Name.LocalName}"" found in a group. Groups can only contain subgroups.", GetErrorPositionInfo(child));
            }
        }
    }
}
