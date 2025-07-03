using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Model.Enums;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastButton", NotificationType.Toast)]
    internal class Action : AdaptiveChildElement, IActivatableElement
    {
        public Action(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const String ATTR_CONTENT = "content";
        internal const String ATTR_IMAGEURI = "imageUri";
        internal const String ATTR_HINT_INPUTID = "hint-inputId";
        internal const String ATTR_PLACEMENT = "placement";

        public String Id { get; set; }

        public ActionPlacement Placement { get; set; }

        public String Content { get; set; }

        public String Arguments { get; set; }

        [ObjectModelProperty("ActivationType")]
        public ActivationType ActivationType { get; set; } = ActivationType.Foreground;

        [ObjectModelProperty("ImageUri")]
        public String ImageUri { get; set; }

        [ObjectModelProperty("InputId")]
        public String InputId { get; set; }

        public Boolean HintVisible { get; set; } = true;

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(node, attributes, result);

            this.HandleRemainingAttributes(attributes, result);
        }

        internal virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // content is required
            XAttribute attrContent = attributes.PopAttribute(ATTR_CONTENT);
            if (attrContent == null)
            {
                result.AddError("content attribute on action element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Content = attrContent.Value;

            // parse activationType, arguments, state
            base.ParseActivatableElementAttributes(node, attributes, result);


            // imageUri is optional
            var attrImageUri = attributes.PopAttribute(ATTR_IMAGEURI);
            if (attrImageUri != null)
                this.ImageUri = attrImageUri.Value;

            // inputId is optional
            var attrInputId = attributes.PopAttribute(ATTR_HINT_INPUTID);
            if (attrInputId != null)
                this.InputId = attrInputId.Value;

            ActionPlacement placement;
            if (this.TryParseEnum(result, attributes, ATTR_PLACEMENT, out placement))
                this.Placement = placement;
        }

        protected override Array GetSupportedEnums<TEnum>()
        {
            // Override the allowed text placement values, since they depend on OS version / supported features
            if (typeof(TEnum) == typeof(ActionPlacement))
                switch (this.Context)
                {
                    case NotificationType.Toast:

                        if (this.SupportedFeatures.ToastContextMenu)
                            return new ActionPlacement[] { ActionPlacement.Inline, ActionPlacement.ContextMenu };

                        return new ActionPlacement[] { ActionPlacement.Inline };



                    default:
                        return new ActionPlacement[] { ActionPlacement.Inline };
                }

            return base.GetSupportedEnums<TEnum>();
        }

        public override ObjectModelObject ConvertToObject()
        {
            var obj = base.ConvertToObject();

            obj.ConstructorValues.Add(new ObjectModelString(this.Content));
            obj.ConstructorValues.Add(new ObjectModelString(this.Arguments));

            switch (this.Placement)
            {
                case ActionPlacement.ContextMenu:
                    obj.Name = "ToastContextMenuItem";
                    break;

                default:
                    switch (this.ActivationType)
                    {
                        case ActivationType.System:
                            obj.PropertyValues.Clear();
                            var args = this.Arguments != null ? this.Arguments : "";
                            switch (args.ToLower())
                            {
                                case "snooze":
                                    obj = new ObjectModelObject("ToastButtonSnooze");
                                    if (!String.IsNullOrWhiteSpace(this.Content))
                                    {
                                        obj.ConstructorValues.Add(new ObjectModelString(this.Content));
                                    }
                                    if (this.InputId != null)
                                    {
                                        obj.Add("SelectionBoxId", new ObjectModelString(this.InputId));
                                    }
                                    break;

                                default:
                                    obj = new ObjectModelObject("ToastButtonDismiss");
                                    if (!String.IsNullOrWhiteSpace(this.Content))
                                    {
                                        obj.ConstructorValues.Add(new ObjectModelString(this.Content));
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }

            return obj;
        }
    }
}
