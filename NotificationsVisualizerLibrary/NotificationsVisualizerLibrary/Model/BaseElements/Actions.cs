using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    [ObjectModelClass("ToastActionsCustom", NotificationType.Toast)]
    internal class Actions : AdaptiveParentElement
    {
        public Actions(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const string ATTR_HINT_SYSTEMCOMMANDS = "hint-systemCommands";

        internal HintSystemCommands HintSystemCommands { get; private set; } = HintSystemCommands.None;

        internal IList<Action> ActionElements { get; private set; } = new List<Action>();

        [ObjectModelProperty("Inputs")]
        public IList<Input> Inputs { get; private set; } = new List<Input>();

        [ObjectModelProperty("Buttons")]
        public IEnumerable<Action> ButtonsForObjectModel
        {
            get
            {
                return ActionElements.Where(i => i.Placement == ActionPlacement.Inline);
            }
        }

        [ObjectModelProperty("ContextMenuItems")]
        public IEnumerable<Action> ContextMenuItems
        {
            get
            {
                return ActionElements.Where(i => i.Placement == ActionPlacement.ContextMenu);
            }
        }

        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            AttributesHelper attributes = new AttributesHelper(node.Attributes());



            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);

            // 0-n children
            foreach (XElement child in node.Elements())
            {
                if (!result.IsOkForRender())
                    break;

                try
                {
                    HandleChild(result, child);
                }

                catch (IncompleteElementException)
                {

                }
            }
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // hint-systemCommands is optional
            HintSystemCommands hintSystemCommands;
            if (TryParseEnum(result, attributes, ATTR_HINT_SYSTEMCOMMANDS, out hintSystemCommands))
                this.HintSystemCommands = hintSystemCommands;
        }

        protected virtual void HandleChild(ParseResult result, XElement child)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "input":

                    var input = new Input(Context, SupportedFeatures);
                    input.Parse(result, child);

                    if (!result.IsOkForRender())
                        return;

                    if (Context == NotificationType.Toast)
                    {
                        if (Inputs.Count >= 5)
                        {
                            result.AddErrorButRenderAllowed("Toasts can only display up to 5 inputs.", GetErrorPositionInfo(child));
                            return;
                        }
                    }

                    Inputs.Add(input);
                    input.Parent = this;

                    break;


                case "action":

                    var action = new Action(Context, SupportedFeatures);
                    action.Parse(result, child);

                    if (!result.IsOkForRender())
                        return;

                    if (Context == NotificationType.Toast)
                    {
                        if (ActionElements.Count >= 5)
                        {
                            result.AddErrorButRenderAllowed("Toasts can only display up to 5 actions.", GetErrorPositionInfo(child));
                            return;
                        }
                    }

                    ActionElements.Add(action);
                    action.Parent = this;

                    break;


                default:
                    result.AddWarning($"Invalid child {child.Name.LocalName} under actions element.", GetErrorPositionInfo(child));
                    break;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { ATTR_HINT_SYSTEMCOMMANDS };
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            foreach (var i in Inputs)
                yield return i;

            foreach (var a in ActionElements)
                yield return a;
        }

        public override ObjectModelObject ConvertToObject()
        {
            switch (HintSystemCommands)
            {
                case HintSystemCommands.SnoozeAndDismiss:
                    return null;

                case HintSystemCommands.None:
                default:
                    return base.ConvertToObject();
            }
        }
    }
}
