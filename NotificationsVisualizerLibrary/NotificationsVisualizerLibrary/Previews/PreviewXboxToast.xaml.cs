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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewXboxToast : UserControl, IPreviewToast
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();

        public PreviewXboxToast()
        {
            this.InitializeComponent();

            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);
        }

        private DataBindingValues _lastDataBindingValues;
        public ParseResult Initialize(XmlDocument content, PreviewNotificationData data)
        {
            ParseResult result = _parser.ParseToast(content.GetXml(), CurrFeatureSet);

            if (result.IsOkForRender())
            {
                _lastDataBindingValues = new DataBindingValues(data);
                result.Toast.ApplyDataBinding(_lastDataBindingValues);

                if (result.IsOkForRender())
                {
                    InitializeContent(result.Toast);
                }
            }

            return result;
        }

        private string _currLaunch = "";
        private ActivationType _currActivationType = ActivationType.Foreground;
        private Dictionary<string, FrameworkElement> _elementsWithIds;

        private IToast _currContent;

        public bool HasContent { get; private set; }

        private void ReInitializeContent()
        {
            InitializeContent(_currContent);
        }

        private void InitializeContent(IToast toastContent)
        {
            HasContent = false;

            TextBlockTitle.Text = "";
            TextBlockSubtitle.Text = "";
            TextBlockSubtitle.Visibility = Visibility.Collapsed;

            ImageAppLogoOverride.Visibility = Visibility.Collapsed;
            CircleImageAppLogoOverride.Visibility = Visibility.Collapsed;

            if (toastContent != null)
            {
                if (toastContent.Visual != null)
                {
                    var visual = toastContent.Visual;

                    var binding = visual.Bindings.FirstOrDefault();

                    if (binding != null)
                    {
                        HasContent = true;

                        var container = binding.Container;

                        var texts = container.Children.OfType<AdaptiveTextField>().ToList();

                        var titleText = texts.ElementAtOrDefault(0);
                        if (titleText != null)
                        {
                            TextBlockTitle.Text = titleText.Text;
                        }

                        var bodyTextLine1 = texts.ElementAtOrDefault(1);
                        if (bodyTextLine1 != null)
                        {
                            TextBlockSubtitle.Text = bodyTextLine1.Text;
                            TextBlockSubtitle.Visibility = Visibility.Visible;
                        }

                        var appLogoOverride = container.Children.OfType<AdaptiveImage>().FirstOrDefault(i => i.Placement == Model.Enums.Placement.AppLogoOverride);
                        if (appLogoOverride != null)
                        {
                            var bmp = ImageHelper.GetBitmap(appLogoOverride.Src);
                            if (appLogoOverride.HintCrop == Model.Enums.HintCrop.Circle)
                            {
                                CircleImageAppLogoOverride.Source = bmp;
                                CircleImageAppLogoOverride.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageAppLogoOverride.Source = bmp;
                                ImageAppLogoOverride.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewToastProperties), typeof(PreviewXboxToast), new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get { return GetValue(PropertiesProperty) as PreviewToastProperties; }
            set { SetValue(PropertiesProperty, value); }
        }

        #region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewXboxToast), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

        /// <summary>
        /// Gets or sets the current device family, which impacts what features are available on the tiles.
        /// </summary>
        public DeviceFamily DeviceFamily
        {
            get { return (DeviceFamily)GetValue(DeviceFamilyProperty); }
            set { SetValue(DeviceFamilyProperty, value); }
        }

        private static void OnDeviceFamilyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewXboxToast).OnDeviceFamilyChanged(e);
        }

        private void OnDeviceFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Feature set is affected
            UpdateFeatureSet();
        }

        #endregion

        private static readonly DependencyProperty OSBuildNumberProperty = DependencyProperty.Register("OSBuildNumber", typeof(Version), typeof(PreviewXboxToast), new PropertyMetadata(FeatureSet.GetCurrentOSBuildNumber(), OnOSBuildNumberChanged));

        /// <summary>
        /// Gets or sets the current OS version, which impacts what features and bug fixes are available.
        /// </summary>
        public int OSBuildNumber
        {
            get { return (int)GetValue(OSBuildNumberProperty); }
            set { SetValue(OSBuildNumberProperty, value); }
        }

        private static void OnOSBuildNumberChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewXboxToast).OnOSBuildNumberChanged(e);
        }

        private void OnOSBuildNumberChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateFeatureSet();
        }

        internal FeatureSet CurrFeatureSet { get; private set; }

        private void UpdateFeatureSet()
        {
            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            this.ReInitializeContent();
        }

        public ParseResult Initialize(XmlDocument content)
        {
            return Initialize(content, null);
        }

        public void Update(PreviewNotificationData data)
        {
            // No-op for now
        }
    }
}
