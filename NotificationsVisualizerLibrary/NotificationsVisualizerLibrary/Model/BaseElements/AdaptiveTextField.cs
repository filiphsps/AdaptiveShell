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
    [ObjectModelClass("AdaptiveText")]
    internal class AdaptiveTextField : AdaptiveChildElement, IBindingChild
    {
        public AdaptiveTextField(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private const string ATTR_TEXT_ID = "id";
        private static readonly string ATTR_TEXT_LANG = "lang";
        private static readonly string ATTR_TEXT_HINT_STYLE = "hint-style";
        private static readonly string ATTR_TEXT_HINT_WRAP = "hint-wrap";
        private static readonly string ATTR_TEXT_HINT_MAX_LINES = "hint-maxLines";
        private static readonly string ATTR_TEXT_HINT_MIN_LINES = "hint-minLines";
        private static readonly string ATTR_TEXT_HINT_ALIGN = "hint-align";
        private const string ATTR_TEXT_PLACEMENT = "placement";

        public string Id { get; set; }

        private string _text = "";
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        [ObjectModelBindingProperty("Text", "BindableString")]
        public string BindingText { get; set; }

        [ObjectModelProperty("Text", null)]
        public IObjectModelValue PropertyText
        {
            get
            {
                if (BindingText != null)
                {
                    return null;
                }

                if (Text == null)
                {
                    return null;
                }

                return new ObjectModelString(Text);
            }
        }

        [ObjectModelProperty("Language")]
        public string Language { get; set; }
        
        public TextPlacement Placement { get; set; }

        [ObjectModelProperty("HintStyle", HintStyle.Default)]
        public HintStyle HintStyle { get; set; } = HintStyle.Default;

        [ObjectModelProperty("HintWrap")]
        public bool? HintWrap { get; set; }

        private int? _hintMaxLines;
        [ObjectModelProperty("HintMaxLines")]
        public int? HintMaxLines
        {
            get { return _hintMaxLines; }

            set
            {
                if (value != null && value.Value < 1)
                    _hintMaxLines = 1;

                else
                    _hintMaxLines = value;
            }
        }

        private int? _hintMinLines;
        [ObjectModelProperty("HintMinLines")]
        public int? HintMinLines
        {
            get { return _hintMinLines; }

            set
            {
                if (value < 1)
                    _hintMinLines = 1;

                else if (value > 10)
                    _hintMinLines = 10;

                else
                    _hintMinLines = value;
            }
        }

        [ObjectModelProperty("HintAlign", HintAlign.Default)]
        public HintAlign HintAlign { get; set; } = HintAlign.Default;

        internal void Parse(ParseResult result, XElement node, bool isBindingRootLevel)
        {
            // Inner text
            // Binding only supported on top level toast elements
            if (isBindingRootLevel && Context == NotificationType.Toast && SupportedFeatures.ToastTextDataBinding)
            {
                string bindingText;
                TryProcessBindableValue(result, node.Value, out bindingText, (val) =>
                {
                    Text = val;
                });
                BindingText = bindingText;
            }
            else
            {
                Text = node.Value;
            }

            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(attributes, result, isBindingRootLevel);

            HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(AttributesHelper attributes, ParseResult result, bool isBindingRootLevel)
        {
            // Max lines is supported on non-toasts, and adaptive toasts, and group/subgroups
            if (Context != NotificationType.Toast || SupportedFeatures.AdaptiveToasts || !isBindingRootLevel)
            {
                // hint-max-lines is optional
                int hintMaxLines;
                if (TryParse(result, attributes, ATTR_TEXT_HINT_MAX_LINES, out hintMaxLines))
                    this.HintMaxLines = hintMaxLines;
            }

            // These features are supported on non-toasts, and group/subgroups
            if (Context != NotificationType.Toast || !isBindingRootLevel)
            {
                // hint-align is optional
                HintAlign hintAlign;
                if (TryParseEnum(result, attributes, ATTR_TEXT_HINT_ALIGN, out hintAlign))
                    this.HintAlign = hintAlign;

                // hint-min-lines is optional
                int hintMinLines;
                if (TryParse(result, attributes, ATTR_TEXT_HINT_MIN_LINES, out hintMinLines))
                    this.HintMinLines = hintMinLines;

                // hint-style is optional
                HintStyle hintStyle;
                if (TryParseEnum(result, attributes, ATTR_TEXT_HINT_STYLE, out hintStyle))
                    this.HintStyle = hintStyle;

                // hint-wrap is optional
                bool hintWrap;
                if (TryParse(result, attributes, ATTR_TEXT_HINT_WRAP, out hintWrap))
                    this.HintWrap = hintWrap;
            }

            if (Context == NotificationType.Tile)
            {
                Id = attributes.PopAttributeValue(ATTR_TEXT_ID);
            }

            TextPlacement placement;
            if (TryParseEnum(result, attributes, ATTR_TEXT_PLACEMENT, out placement))
                this.Placement = placement;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { ATTR_TEXT_LANG };
        }

        protected override Array GetSupportedEnums<TEnum>()
        {
            // Override the allowed text placement values, since they depend on OS version / supported features
            if (typeof(TEnum) == typeof(TextPlacement))
            {
                switch (Context)
                {
                    case NotificationType.Toast:

                        if (SupportedFeatures.ToastAttribution)
                            return new TextPlacement[] { TextPlacement.Inline, TextPlacement.Attribution };

                        return new TextPlacement[] { TextPlacement.Inline };



                    default:
                        return new TextPlacement[] { TextPlacement.Inline };
                }
            }

            return base.GetSupportedEnums<TEnum>();
        }

        public override ObjectModelObject ConvertToObject()
        {
            switch (Placement)
            {
                case TextPlacement.Attribution:
                    return new ObjectModelObject("ToastGenericAttributionText")
                    {
                        { "Text", Text },
                        { "Language", Language }
                    };

                default:
                    return base.ConvertToObject();
            }
        }
    }
}
