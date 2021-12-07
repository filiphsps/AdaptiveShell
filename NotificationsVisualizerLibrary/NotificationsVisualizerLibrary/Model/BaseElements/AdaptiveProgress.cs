using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    [ObjectModelClass("AdaptiveProgressBar")]
    internal class AdaptiveProgress : AdaptiveChildElement, IBindingChild
    {
        public AdaptiveProgress(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private static readonly string ATTR_TITLE = "title";
        private static readonly string ATTR_VALUE_STRING_OVERRIDE = "valueStringOverride";
        private static readonly string ATTR_VALUE = "value";
        private static readonly string ATTR_STATUS = "status";

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[0];
        }

        private string _title;
        /// <summary>
        /// Optional
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        [ObjectModelBindingProperty("Value", "BindableProgressBarValue")]
        public string BindingValue { get; set; }

        [ObjectModelBindingProperty("ValueStringOverride", "BindableString")]
        public string BindingValueStringOverride { get; set; }

        [ObjectModelBindingProperty("Title", "BindableString")]
        public string BindingTitle { get; set; }

        [ObjectModelBindingProperty("Status", "BindableString")]
        public string BindingStatus { get; set; }

        [ObjectModelProperty("Value", null)]
        public IObjectModelValue PropertyValue
        {
            get
            {
                if (BindingValue != null)
                {
                    // If data binding being used, the other property will pick it up
                    return null;
                }

                if (IsIndeterminate)
                {
                    return new ObjectModelEnum("AdaptiveProgressBarValue", "Intermediate");
                }

                return new ObjectModelObjectFromStaticMethod("AdaptiveProgressBarValue", "FromValue", new ObjectModelDouble(Value));
            }
        }

        [ObjectModelProperty("ValueStringOverride", null)]
        public IObjectModelValue PropertyValueStringOverride
        {
            get
            {
                if (BindingValueStringOverride != null)
                {
                    return null;
                }

                if (ValueStringOverride == null)
                {
                    return null;
                }

                return new ObjectModelString(ValueStringOverride);
            }
        }

        [ObjectModelProperty("Title", null)]
        public IObjectModelValue PropertyTitle
        {
            get
            {
                if (BindingTitle != null)
                {
                    return null;
                }

                if (Title == null)
                {
                    return null;
                }

                return new ObjectModelString(Title);
            }
        }

        [ObjectModelProperty("Status", null)]
        public IObjectModelValue PropertyStatus
        {
            get
            {
                if (BindingStatus != null)
                {
                    return null;
                }

                if (Status == null)
                {
                    return null;
                }

                return new ObjectModelString(Status);
            }
        }

        private double _value;
        /// <summary>
        /// Required, value between 0.0 and 1.0
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); NotifyPropertyChanged(nameof(ValueString)); }
        }

        public string ValueString
        {
            get
            {
                if (ValueStringOverride != null)
                {
                    return ValueStringOverride;
                }

                return (Value).ToString("0%");
            }
        }

        private bool _isIndeterminate;
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { SetProperty(ref _isIndeterminate, value); }
        }

        private string _valueStringOverride;
        public string ValueStringOverride
        {
            get { return _valueStringOverride; }
            set { SetProperty(ref _valueStringOverride, value); NotifyPropertyChanged(nameof(ValueString)); }
        }

        private string _status;
        /// <summary>
        /// Optional
        /// </summary>
        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // title is optional
            string bindingTitle;
            TryPopAttributeValueWithBinding(result, attributes, ATTR_TITLE, out bindingTitle, (val) =>
            {
                Title = val;
            });
            BindingTitle = bindingTitle;

            // valueStringOverride is optional
            string bindingValueStringOverride;
            TryPopAttributeValueWithBinding(result, attributes, ATTR_VALUE_STRING_OVERRIDE, out bindingValueStringOverride, (val) =>
            {
                ValueStringOverride = val;
            });
            BindingValueStringOverride = bindingValueStringOverride;

            // value is required
            string bindingValue;
            bool hadValue = TryPopAttributeValueWithBinding(result, attributes, ATTR_VALUE, out bindingValue, (val) =>
            {
                if (val == null)
                {
                    throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value wasn't provided.", XmlTemplateParser.GetErrorPositionInfo(node)));
                }

                double value = 0;
                if (val.Equals("indeterminate", StringComparison.CurrentCultureIgnoreCase))
                {
                    IsIndeterminate = true;
                }
                else if (!double.TryParse(val, out value))
                {
                    throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value must be a double between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node)));
                }
                else
                {
                    if (value < 0 || value > 1)
                    {
                        IsIndeterminate = false;
                        Value = (value < 0) ? 0 : 1;
                        throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value must be between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node)));
                    }

                    IsIndeterminate = false;
                    Value = value;
                }
            });
            if (!hadValue)
            {
                Value = 0;
                result.AddErrorButRenderAllowed("value attribute on progress element is required and must be a double between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node));
            }

            BindingValue = bindingValue;

            // status is optional
            string bindingStatus;
            TryPopAttributeValueWithBinding(result, attributes, ATTR_STATUS, out bindingStatus, (val) =>
            {
                Status = val;
            });
            BindingStatus = bindingStatus;
        }
    }
}
