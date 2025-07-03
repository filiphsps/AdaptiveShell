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

        internal const String ATTR_ID = "id";
        internal const String ATTR_TYPE = "type";
        internal const String ATTR_TITLE = "title";
        internal const String ATTR_PLACEHOLDERCONTENT = "placeHolderContent";
        internal const String ATTR_DEFAULTINPUT = "defaultInput";

        public String Id { get; set; }

        public InputType Type { get; set; }

        [ObjectModelProperty("Title")]
        public String Title { get; set; }

        [ObjectModelProperty("PlaceholderContent")]
        public String PlaceHolderContent { get; set; }

        [ObjectModelProperty("DefaultInput")]
        public String DefaultInput { get; set; }

        public Boolean HintVisible { get; set; } = true;

        internal IList<Selection> Children { get; private set; } = new List<Selection>();


        internal void Parse(ParseResult result, XElement node)
        {
            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(node, attributes, result);

            this.HandleRemainingAttributes(attributes, result);

            foreach (XElement n in node.Elements())
            {
                try
                {
                    this.HandleChild(result, n);
                }

                catch (IncompleteElementException) { }
            }
        }


        protected void HandleChild(ParseResult result, XElement child)
        {
            if (this.Type != InputType.Selection)
            {
                result.AddWarning($"Invalid child '{child.Name.LocalName}' found in an input. Only inputs of type 'selection' can contain children.", GetErrorPositionInfo(child));
                return;
            }

            if (child.IsType("selection"))
            {

                var selection = new Selection(this.Context, this.SupportedFeatures);
                selection.Parse(result, child);

                if (!result.IsOkForRender())
                    throw new IncompleteElementException();

                this.Children.Add(selection);
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
            if (!this.TryParseEnum(result, attributes, ATTR_TYPE, out type))
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

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { };
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return this.Children;
        }

        public override ObjectModelObject ConvertToObject()
        {
            var obj = base.ConvertToObject();

            switch (this.Type)
            {
                case InputType.Selection:
                    return new ObjectModelObject("ToastSelectionBox", new ObjectModelString(this.Id))
                    {
                        { "Title", this.Title },
                        { "DefaultSelectionBoxItemId", this.DefaultInput },
                        { "Items", new ObjectModelListInitialization(this.Children.Select(i => i.ConvertToObject())) }
                    };

                case InputType.Text:
                    obj.ConstructorValues.Add(new ObjectModelString(this.Id));
                    break;
            }

            return obj;
        }
    }
}
