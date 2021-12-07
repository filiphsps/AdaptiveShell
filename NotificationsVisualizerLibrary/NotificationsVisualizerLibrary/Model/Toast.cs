using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model
{
    internal interface IToast
    {
        FeatureSet SupportedFeatures { get; }

        NotificationType Context { get; }

        Header Header { get; set; }

        Visual Visual { get; set; }

        Actions Actions { get; set; }

        Audio Audio { get; set; }

        string Launch { get; set; }

        Scenario Scenario { get; set; }

        ActivationType? ActivationType { get; set; }

        Duration? Duration { get; set; }

        string People { get; set; }
    }

    [ObjectModelClass("ToastContent")]
    internal class Toast : AdaptiveParentElement, IToast
    {
        private const string ATTR_LAUNCH = "launch";
        private const string ATTR_DURATION = "duration";
        private const string ATTR_SCENARIO = "scenario";
        private const string ATTR_HINT_PEOPLE = "hint-people";
        private const string ATTR_DISPLAYTIMESTAMP = "displayTimestamp";

        public DataBindingValues DataBinding { get; private set; }

        public Toast(FeatureSet supportedFeatures) : base(NotificationType.Toast, supportedFeatures)
        {
        }

        [ObjectModelProperty("Header")]
        public Header Header { get; set; }

        [ObjectModelProperty("Visual")]
        public Visual Visual { get; set; }

        [ObjectModelProperty("Actions")]
        public Actions Actions { get; set; }

        [ObjectModelProperty("Audio")]
        public Audio Audio { get; set; }

        [ObjectModelProperty("Launch", "")]
        public string Launch { get; set; } = "";

        [ObjectModelProperty("Scenario", Scenario.Default)]
        public Scenario Scenario { get; set; } = Scenario.Default;

        [ObjectModelProperty("ActivationType")]
        public ActivationType? ActivationType { get; set; }

        [ObjectModelProperty("Duration")]
        public Duration? Duration { get; set; }

        [ObjectModelProperty("DisplayTimestamp")]
        public DateTime? DisplayTimestamp { get; set; }

        public string People { get; set; }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            
            ParseKnownAttributes(attributes, result);

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

        private void ParseKnownAttributes(AttributesHelper attributes, ParseResult result)
        {
            // Launch is optional
            var attrLaunc = attributes.PopAttribute(ATTR_LAUNCH);
            if (attrLaunc != null)
                Launch = attrLaunc.Value;

            // TODO - check duration is valid

            // activationType is optional
            ActivationType activationType;
            if (TryParseEnum<ActivationType>(result, attributes, ATTR_ACTIVATIONTYPE, out activationType))
                ActivationType = activationType;

            // scenario is optional
            Scenario scenario;
            if (TryParseEnum(result, attributes, ATTR_SCENARIO, out scenario))
                Scenario = scenario;

            // duration is optional
            Duration duration;
            if (TryParseEnum(result, attributes, ATTR_DURATION, out duration))
                Duration = duration;

            // hint-people is optional
            var attrPeople = attributes.PopAttribute(ATTR_HINT_PEOPLE);
            if (attrPeople != null)
                People = attrPeople.Value;

            // displayTimestamp is optional
            if (SupportedFeatures.ToastDisplayTimestamp)
            {
                var attrDisplayTimestamp = attributes.PopAttribute(ATTR_DISPLAYTIMESTAMP);
                if (attrDisplayTimestamp != null)
                {
                    try
                    {
                        DisplayTimestamp = System.Xml.XmlConvert.ToDateTime(attrDisplayTimestamp.Value, System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
                    }
                    catch (FormatException)
                    {
                        result.AddWarning(ATTR_DISPLAYTIMESTAMP + " must be specified in ISO 8601 format.", GetErrorPositionInfo(attrDisplayTimestamp));
                    }
                }
            }
        }

        private void HandleChild(ParseResult result, XElement child)
        {
            if (child.IsType("visual"))
            {
                if (Visual != null)
                {
                    result.AddWarning("A visual element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                Visual visual = new Visual(NotificationType.Toast, SupportedFeatures);
                visual.Parse(result, child);

                Visual = visual;
            }

            else if (child.IsType("actions"))
            {
                if (Actions != null)
                {
                    result.AddWarning("An actions element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                Actions actions = new Actions(NotificationType.Toast, SupportedFeatures);
                actions.Parse(result, child);

                Actions = actions;
            }

            else if (child.IsType("audio"))
            {
                if (Audio != null)
                {
                    result.AddWarning("An audio element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                Audio audio = new Audio(NotificationType.Toast, SupportedFeatures);
                audio.Parse(result, child);

                Audio = audio;
            }

            else if (SupportedFeatures.ToastHeaders && child.IsType("header"))
            {
                if (Header != null)
                {
                    result.AddErrorButRenderAllowed("A header element was already provided. Only one header is allowed.", GetErrorPositionInfo(child));
                    return;
                }

                Header header = new Header(NotificationType.Toast, SupportedFeatures);
                header.Parse(result, child);

                Header = header;
            }

            else
                result.AddError($"Invalid child {child.Name.LocalName} under toast element.", GetErrorPositionInfo(child));
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return new AdaptiveChildElement[]
            {
                Visual,
                Actions,
                Audio
            }.Where(i => i != null);
        }
    }
}
