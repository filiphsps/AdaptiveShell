using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewXboxToast : UserControl, IPreviewToast
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();

        public PreviewXboxToast()
        {
            this.InitializeComponent();

            this.CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);
        }

        private DataBindingValues _lastDataBindingValues;
        public ParseResult Initialize(XmlDocument content, PreviewNotificationData data)
        {
            ParseResult result = _parser.ParseToast(content.GetXml(), this.CurrFeatureSet);

            if (result.IsOkForRender())
            {
                this._lastDataBindingValues = new DataBindingValues(data);
                result.Toast.ApplyDataBinding(this._lastDataBindingValues);

                if (result.IsOkForRender())
                {
                    this.InitializeContent(result.Toast);
                }
            }

            return result;
        }

        private String _currLaunch = "";
        private ActivationType _currActivationType = ActivationType.Foreground;
        private Dictionary<String, FrameworkElement> _elementsWithIds;

        private IToast _currContent;

        public Boolean HasContent { get; private set; }

        private void ReInitializeContent()
        {
            this.InitializeContent(this._currContent);
        }

        private void InitializeContent(IToast toastContent)
        {
            this.HasContent = false;

            this.TextBlockTitle.Text = "";
            this.TextBlockSubtitle.Text = "";
            this.TextBlockSubtitle.Visibility = Visibility.Collapsed;

            this.ImageAppLogoOverride.Visibility = Visibility.Collapsed;
            this.CircleImageAppLogoOverride.Visibility = Visibility.Collapsed;

            if (toastContent != null)
            {
                if (toastContent.Visual != null)
                {
                    var visual = toastContent.Visual;

                    var binding = visual.Bindings.FirstOrDefault();

                    if (binding != null)
                    {
                        this.HasContent = true;

                        var container = binding.Container;

                        var texts = container.Children.OfType<AdaptiveTextField>().ToList();

                        var titleText = texts.ElementAtOrDefault(0);
                        if (titleText != null)
                        {
                            this.TextBlockTitle.Text = titleText.Text;
                        }

                        var bodyTextLine1 = texts.ElementAtOrDefault(1);
                        if (bodyTextLine1 != null)
                        {
                            this.TextBlockSubtitle.Text = bodyTextLine1.Text;
                            this.TextBlockSubtitle.Visibility = Visibility.Visible;
                        }

                        var appLogoOverride = container.Children.OfType<AdaptiveImage>().FirstOrDefault(i => i.Placement == Model.Enums.Placement.AppLogoOverride);
                        if (appLogoOverride != null)
                        {
                            var bmp = ImageHelper.GetBitmap(appLogoOverride.Src);
                            if (appLogoOverride.HintCrop == Model.Enums.HintCrop.Circle)
                            {
                                this.CircleImageAppLogoOverride.Source = bmp;
                                this.CircleImageAppLogoOverride.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                this.ImageAppLogoOverride.Source = bmp;
                                this.ImageAppLogoOverride.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewToastProperties), typeof(PreviewXboxToast), new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get { return this.GetValue(PropertiesProperty) as PreviewToastProperties; }
            set { this.SetValue(PropertiesProperty, value); }
        }

        #region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewXboxToast), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

        /// <summary>
        /// Gets or sets the current device family, which impacts what features are available on the tiles.
        /// </summary>
        public DeviceFamily DeviceFamily
        {
            get { return (DeviceFamily)this.GetValue(DeviceFamilyProperty); }
            set { this.SetValue(DeviceFamilyProperty, value); }
        }

        private static void OnDeviceFamilyChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewXboxToast).OnDeviceFamilyChanged(e);
        }

        private void OnDeviceFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Feature set is affected
            this.UpdateFeatureSet();
        }

        #endregion

        private static readonly DependencyProperty OSBuildNumberProperty = DependencyProperty.Register("OSBuildNumber", typeof(Version), typeof(PreviewXboxToast), new PropertyMetadata(FeatureSet.GetCurrentOSBuildNumber(), OnOSBuildNumberChanged));

        /// <summary>
        /// Gets or sets the current OS version, which impacts what features and bug fixes are available.
        /// </summary>
        public Int32 OSBuildNumber
        {
            get { return (Int32)this.GetValue(OSBuildNumberProperty); }
            set { this.SetValue(OSBuildNumberProperty, value); }
        }

        private static void OnOSBuildNumberChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewXboxToast).OnOSBuildNumberChanged(e);
        }

        private void OnOSBuildNumberChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateFeatureSet();
        }

        internal FeatureSet CurrFeatureSet { get; private set; }

        private void UpdateFeatureSet()
        {
            this.CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            this.ReInitializeContent();
        }

        public ParseResult Initialize(XmlDocument content)
        {
            return this.Initialize(content, null);
        }

        public void Update(PreviewNotificationData data)
        {
            // No-op for now
        }
    }
}
