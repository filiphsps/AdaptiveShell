using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastTextBox", NotificationType.Toast)]
    internal class Input : AdaptiveParentElement
    {
        public Input(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const string ATTR_ID = "id";
        internal const string ATTR_TYPE = "type";
        internal const string ATTR_TITLE = "title";
        internal const string ATTR_PLACEHOLDERCONTENT = "placeHolderContent";
        internal const string ATTR_DEFAULTINPUT = "defaultInput";

        public string Id { get; set; }

        public InputType Type { get; set; }

        [ObjectModelProperty("Title")]
        public string Title { get; set; }

        [ObjectModelProperty("PlaceholderContent")]
        public string PlaceHolderContent { get; set; }

        [ObjectModelProperty("DefaultInput")]
        public string DefaultInput { get; set; }

        public bool HintVisible { get; set; } = true;

        internal IList<Selection> Children { get; private set; } = new List<Selection>();


        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);

            foreach (XElement n in node.Elements())
            {
                try
                {
                    HandleChild(result, n);
                }

                catch (IncompleteElementException) { }
            }
        }


        protected void HandleChild(ParseResult result, XElement child)
        {
            if (Type != InputType.Selection)
            {
                result.AddWarning($"Invalid child '{child.Name.LocalName}' found in an input. Only inputs of type 'selection' can contain children.", GetErrorPositionInfo(child));
                return;
            }

            if (child.IsType("selection"))
            {

                Selection selection = new Selection(Context, SupportedFeatures);
                selection.Parse(result, child);

                if (!result.IsOkForRender())
                    throw new IncompleteElementException();

                Children.Add(selection);
                selection.Parent = this;
            }

            else
            {
                result.AddError($@"Invalid child ""{child.Name.LocalName}"" found in an input. Selection inputs can only contain selection elements.", GetErrorPositionInfo(child));
            }
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
            InputType type;
            if (!TryParseEnum(result, attributes, ATTR_TYPE, out type))
            {
                result.AddErrorButRenderAllowed("type attribute on input element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Id = attrId.Value;
            this.Type = type;
            

            // title is optional
            var attrTitle = attributes.PopAttribute(ATTR_TITLE);
            if (attrTitle != null)
                this.Title = attrTitle.Value;

            // placeHolderContent is optional
            var attrPlaceHolderContent = attributes.PopAttribute(ATTR_PLACEHOLDERCONTENT);
            if (attrPlaceHolderContent != null)
                this.PlaceHolderContent = attrPlaceHolderContent.Value;

            // defaultInput is optional
            var attrDefaultInput = attributes.PopAttribute(ATTR_DEFAULTINPUT);
            if (attrDefaultInput != null)
                this.DefaultInput = attrDefaultInput.Value;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return Children;
        }

        public override ObjectModelObject ConvertToObject()
        {
            var obj = base.ConvertToObject();

            switch (Type)
            {
                case InputType.Selection:
                    return new ObjectModelObject("ToastSelectionBox", new ObjectModelString(Id))
                    {
                        { "Title", Title },
                        { "DefaultSelectionBoxItemId", DefaultInput },
                        { "Items", new ObjectModelListInitialization(Children.Select(i => i.ConvertToObject())) }
                    };

                case InputType.Text:
                    obj.ConstructorValues.Add(new ObjectModelString(Id));
                    break;
            }

            return obj;
        }
    }
}
