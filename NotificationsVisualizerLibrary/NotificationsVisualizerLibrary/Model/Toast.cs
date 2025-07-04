using System;
using System.Collections.Generic;
using System.Linq;
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

        String Launch { get; set; }

        Scenario Scenario { get; set; }

        ActivationType? ActivationType { get; set; }

        Duration? Duration { get; set; }

        String People { get; set; }
    }

    [ObjectModelClass("ToastContent")]
    internal partial class Toast : AdaptiveParentElement, IToast
    {
        private const String ATTR_LAUNCH = "launch";
        private const String ATTR_DURATION = "duration";
        private const String ATTR_SCENARIO = "scenario";
        private const String ATTR_HINT_PEOPLE = "hint-people";
        private const String ATTR_DISPLAYTIMESTAMP = "displayTimestamp";

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
        public String Launch { get; set; } = "";

        [ObjectModelProperty("Scenario", Scenario.Default)]
        public Scenario Scenario { get; set; } = Scenario.Default;

        [ObjectModelProperty("ActivationType")]
        public ActivationType? ActivationType { get; set; }

        [ObjectModelProperty("Duration")]
        public Duration? Duration { get; set; }

        [ObjectModelProperty("DisplayTimestamp")]
        public DateTime? DisplayTimestamp { get; set; }

        public String People { get; set; }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            if (!XmlTemplateParser.EnsureNodeOnlyHasElementsAsChildren(result, node))
                throw new IncompleteElementException();

            var attributes = new AttributesHelper(node.Attributes());


            this.ParseKnownAttributes(attributes, result);

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

        private void ParseKnownAttributes(AttributesHelper attributes, ParseResult result)
        {
            // Launch is optional
            var attrLaunc = attributes.PopAttribute(ATTR_LAUNCH);
            if (attrLaunc != null)
                this.Launch = attrLaunc.Value;

            // TODO - check duration is valid

            // activationType is optional
            ActivationType activationType;
            if (this.TryParseEnum(result, attributes, ATTR_ACTIVATIONTYPE, out activationType))
                this.ActivationType = activationType;

            // scenario is optional
            Scenario scenario;
            if (this.TryParseEnum(result, attributes, ATTR_SCENARIO, out scenario))
                this.Scenario = scenario;

            // duration is optional
            Duration duration;
            if (this.TryParseEnum(result, attributes, ATTR_DURATION, out duration))
                this.Duration = duration;

            // hint-people is optional
            var attrPeople = attributes.PopAttribute(ATTR_HINT_PEOPLE);
            if (attrPeople != null)
                this.People = attrPeople.Value;

            // displayTimestamp is optional
            if (this.SupportedFeatures.ToastDisplayTimestamp)
            {
                var attrDisplayTimestamp = attributes.PopAttribute(ATTR_DISPLAYTIMESTAMP);
                if (attrDisplayTimestamp != null)
                {
                    try
                    {
                        this.DisplayTimestamp = System.Xml.XmlConvert.ToDateTime(attrDisplayTimestamp.Value, System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
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
                if (this.Visual != null)
                {
                    result.AddWarning("A visual element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                var visual = new Visual(NotificationType.Toast, this.SupportedFeatures);
                visual.Parse(result, child);

                this.Visual = visual;
            }

            else if (child.IsType("actions"))
            {
                if (this.Actions != null)
                {
                    result.AddWarning("An actions element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                var actions = new Actions(NotificationType.Toast, this.SupportedFeatures);
                actions.Parse(result, child);

                this.Actions = actions;
            }

            else if (child.IsType("audio"))
            {
                if (this.Audio != null)
                {
                    result.AddWarning("An audio element was already provided. Only the first one will be used.", GetErrorPositionInfo(child));
                    return;
                }

                var audio = new Audio(NotificationType.Toast, this.SupportedFeatures);
                audio.Parse(result, child);

                this.Audio = audio;
            }

            else if (this.SupportedFeatures.ToastHeaders && child.IsType("header"))
            {
                if (this.Header != null)
                {
                    result.AddErrorButRenderAllowed("A header element was already provided. Only one header is allowed.", GetErrorPositionInfo(child));
                    return;
                }

                var header = new Header(NotificationType.Toast, this.SupportedFeatures);
                header.Parse(result, child);

                this.Header = header;
            }

            else
                result.AddError($"Invalid child {child.Name.LocalName} under toast element.", GetErrorPositionInfo(child));
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return new AdaptiveChildElement[]
            {
                this.Visual,
                this.Actions,
                this.Audio
            }.Where(i => i != null);
        }
    }
}
