using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Parsers;
using System;

namespace NotificationsVisualizerLibrary.Renderers
{
    /// <summary>
    /// A control that renders adaptive content.
    /// </summary>
    [TemplatePart(Name = "PART_AdaptiveContainer", Type = typeof(Border))]
    public sealed partial class PreviewAdaptiveContent : Control
    {
        private static XmlTemplateParser _parser = new();
        private Border _adaptiveContainer;

        public Double ExternalMargin { get; set; } = 8;

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

            this._adaptiveContainer = this.GetTemplateChild("PART_AdaptiveContainer") as Border;

            if (this._adaptiveContainer == null)
                throw new NullReferenceException("Template parts not available");
        }
        
        /// <summary>
        /// Display the adaptive content
        /// </summary>
        /// <param name="adaptiveContent"></param>
        public ParseResult Initialize(String adaptiveContent)
        {
            ParseResult result = _parser.ParseAdaptiveContent(adaptiveContent, FeatureSet.Get(FeatureSet.GetCurrentDeviceFamily(), FeatureSet.GetCurrentOSBuildNumber()));

            if (result.IsOkForRender())
            {
                this.InitializeContent(result.AdaptiveContent);
            }

            return result;
        }

        private void InitializeContent(AdaptiveContainer adaptiveContent)
        {
            this.Reset();

            this._adaptiveContainer.Child = AdaptiveRenderer.Render(adaptiveContent, new(this.GetExternalMargin()));
        }

        private void Reset()
        {
            this._adaptiveContainer.Child = null;
        }

        private Double GetExternalMargin()
        {
            return this.ExternalMargin;
        }
    }
}
