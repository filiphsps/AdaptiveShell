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

        private static readonly String ATTR_TITLE = "title";
        private static readonly String ATTR_VALUE_STRING_OVERRIDE = "valueStringOverride";
        private static readonly String ATTR_VALUE = "value";
        private static readonly String ATTR_STATUS = "status";

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[0];
        }

        private String _title;
        /// <summary>
        /// Optional
        /// </summary>
        public String Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        [ObjectModelBindingProperty("Value", "BindableProgressBarValue")]
        public String BindingValue { get; set; }

        [ObjectModelBindingProperty("ValueStringOverride", "BindableString")]
        public String BindingValueStringOverride { get; set; }

        [ObjectModelBindingProperty("Title", "BindableString")]
        public String BindingTitle { get; set; }

        [ObjectModelBindingProperty("Status", "BindableString")]
        public String BindingStatus { get; set; }

        [ObjectModelProperty("Value", null)]
        public IObjectModelValue PropertyValue
        {
            get
            {
                if (this.BindingValue != null)
                {
                    // If data binding being used, the other property will pick it up
                    return null;
                }

                if (this.IsIndeterminate)
                {
                    return new ObjectModelEnum("AdaptiveProgressBarValue", "Intermediate");
                }

                return new ObjectModelObjectFromStaticMethod("AdaptiveProgressBarValue", "FromValue", new ObjectModelDouble(this.Value));
            }
        }

        [ObjectModelProperty("ValueStringOverride", null)]
        public IObjectModelValue PropertyValueStringOverride
        {
            get
            {
                if (this.BindingValueStringOverride != null)
                {
                    return null;
                }

                if (this.ValueStringOverride == null)
                {
                    return null;
                }

                return new ObjectModelString(this.ValueStringOverride);
            }
        }

        [ObjectModelProperty("Title", null)]
        public IObjectModelValue PropertyTitle
        {
            get
            {
                if (this.BindingTitle != null)
                {
                    return null;
                }

                if (this.Title == null)
                {
                    return null;
                }

                return new ObjectModelString(this.Title);
            }
        }

        [ObjectModelProperty("Status", null)]
        public IObjectModelValue PropertyStatus
        {
            get
            {
                if (this.BindingStatus != null)
                {
                    return null;
                }

                if (this.Status == null)
                {
                    return null;
                }

                return new ObjectModelString(this.Status);
            }
        }

        private Double _value;
        /// <summary>
        /// Required, value between 0.0 and 1.0
        /// </summary>
        public Double Value
        {
            get { return this._value; }
            set { this.SetProperty(ref this._value, value); this.NotifyPropertyChanged(nameof(this.ValueString)); }
        }

        public String ValueString
        {
            get
            {
                if (this.ValueStringOverride != null)
                {
                    return this.ValueStringOverride;
                }

                return (this.Value).ToString("0%");
            }
        }

        private Boolean _isIndeterminate;
        public Boolean IsIndeterminate
        {
            get { return this._isIndeterminate; }
            set { this.SetProperty(ref this._isIndeterminate, value); }
        }

        private String _valueStringOverride;
        public String ValueStringOverride
        {
            get { return this._valueStringOverride; }
            set { this.SetProperty(ref this._valueStringOverride, value); this.NotifyPropertyChanged(nameof(this.ValueString)); }
        }

        private String _status;
        /// <summary>
        /// Optional
        /// </summary>
        public String Status
        {
            get { return this._status; }
            set { this.SetProperty(ref this._status, value); }
        }

        internal void Parse(ParseResult result, XElement node)
        {
            var attributes = new AttributesHelper(node.Attributes());

            this.ParseKnownAttributes(node, attributes, result);

            this.HandleRemainingAttributes(attributes, result);
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // title is optional
            String bindingTitle;
            this.TryPopAttributeValueWithBinding(result, attributes, ATTR_TITLE, out bindingTitle, (val) =>
            {
                this.Title = val;
            });
            this.BindingTitle = bindingTitle;

            // valueStringOverride is optional
            String bindingValueStringOverride;
            this.TryPopAttributeValueWithBinding(result, attributes, ATTR_VALUE_STRING_OVERRIDE, out bindingValueStringOverride, (val) =>
            {
                this.ValueStringOverride = val;
            });
            this.BindingValueStringOverride = bindingValueStringOverride;

            // value is required
            String bindingValue;
            Boolean hadValue = this.TryPopAttributeValueWithBinding(result, attributes, ATTR_VALUE, out bindingValue, (val) =>
            {
                if (val == null)
                {
                    throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value wasn't provided.", XmlTemplateParser.GetErrorPositionInfo(node)));
                }

                Double value = 0;
                if (val.Equals("indeterminate", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.IsIndeterminate = true;
                }
                else if (!Double.TryParse(val, out value))
                {
                    throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value must be a double between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node)));
                }
                else
                {
                    if (value < 0 || value > 1)
                    {
                        this.IsIndeterminate = false;
                        this.Value = (value < 0) ? 0 : 1;
                        throw new ParseErrorException(new ParseError(ParseErrorType.ErrorButRenderAllowed, "progress value must be between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node)));
                    }

                    this.IsIndeterminate = false;
                    this.Value = value;
                }
            });
            if (!hadValue)
            {
                this.Value = 0;
                result.AddErrorButRenderAllowed("value attribute on progress element is required and must be a double between 0.0 and 1.0, or 'indeterminate'.", XmlTemplateParser.GetErrorPositionInfo(node));
            }

            this.BindingValue = bindingValue;

            // status is optional
            String bindingStatus;
            this.TryPopAttributeValueWithBinding(result, attributes, ATTR_STATUS, out bindingStatus, (val) =>
            {
                this.Status = val;
            });
            this.BindingStatus = bindingStatus;
        }
    }
}
