using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Helpers;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationsVisualizerLibrary.Renderers
{
    /// <summary>
    /// A control that renders adaptive content.
    /// </summary>
    [TemplatePart(Name = "PART_AdaptiveContainer", Type = typeof(Border))]
    public sealed class PreviewAdaptiveContent : Control
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();
        private Border _adaptiveContainer;

        public double ExternalMargin { get; set; } = 8;

        /// <summary>
        /// Constructs a new PreviewAdaptiveContent control.
        /// </summary>
        public PreviewAdaptiveContent()
        {
            this.DefaultStyleKey = typeof(PreviewAdaptiveContent);
        }

        /// <summary>
        /// Obtain the AdaptiveStackPanel root
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _adaptiveContainer = GetTemplateChild("PART_AdaptiveContainer") as Border;

            if (_adaptiveContainer == null)
                throw new NullReferenceException("Template parts not available");
        }
        
        /// <summary>
        /// Display the adaptive content
        /// </summary>
        /// <param name="adaptiveContent"></param>
        public ParseResult Initialize(string adaptiveContent)
        {
            ParseResult result = _parser.ParseAdaptiveContent(adaptiveContent, FeatureSet.Get(FeatureSet.GetCurrentDeviceFamily(), FeatureSet.GetCurrentOSBuildNumber()));

            if (result.IsOkForRender())
            {
                InitializeContent(result.AdaptiveContent);
            }

            return result;
        }

        private void InitializeContent(AdaptiveContainer adaptiveContent)
        {
            Reset();

            _adaptiveContainer.Child = AdaptiveRenderer.Render(adaptiveContent, new Thickness(GetExternalMargin()));
        }

        private void Reset()
        {
            _adaptiveContainer.Child = null;
        }

        private double GetExternalMargin()
        {
            return ExternalMargin;
        }
    }
}
