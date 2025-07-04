using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewXboxToastInActionCenter : UserControl, IPreviewToast
    {
        private static XmlTemplateParser _parser = new();

        private readonly ObservableCollection<ListViewButton> _buttons = new();

        public event EventHandler<PreviewToastNotificationActivatedEventArgs> ActivatedForeground, ActivatedBackground, ActivatedSystem, ActivatedProtocol;

        public PreviewXboxToastInActionCenter()
        {
            this.InitializeComponent();

            this.CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            this.ListViewButtons.ItemsSource = this._buttons;
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
            this._currContent = toastContent;
            this.HasContent = false;

            this.TextBlockTitle.Text = "";
            this.TextBlockTitle.MaxLines = 3;
            this.TextBlockTitle.TextWrapping = TextWrapping.WrapWholeWords;

            this.TextBlockBody.Text = "";
            this.TextBlockBody.MaxLines = 3;
            this.TextBlockBody.TextWrapping = TextWrapping.WrapWholeWords;

            this.TextBlockBody2.Visibility = Visibility.Collapsed;
            this.TextBlockBody2.MaxLines = 3;
            this.TextBlockBody2.TextWrapping = TextWrapping.WrapWholeWords;

            this.StackPanelInlineImages.Children.Clear();

            this.DefaultImageAppLogo.Visibility = Visibility.Visible;
            this.ImageAppLogo.Visibility = Visibility.Collapsed;
            this.CircleImageAppLogo.Visibility = Visibility.Collapsed;

            this._elementsWithIds = [];

            this._currLaunch = "";
            this._currActivationType = ActivationType.Foreground;

            this._buttons.Clear();
            this._buttons.Add(this.CreateButton("Launch", toastContent?.Launch, null, toastContent != null ? toastContent.ActivationType.GetValueOrDefault(ActivationType.Foreground) : ActivationType.Foreground, Scenario.Default));



            if (toastContent != null)
            {
                this._currLaunch = toastContent.Launch;
                this._currActivationType = toastContent.ActivationType.GetValueOrDefault(ActivationType.Foreground);

                if (toastContent.Visual != null)
                {
                    var visual = toastContent.Visual;

                    AdaptiveBinding binding;

                    if (toastContent.Context == NotificationType.Toast)
                        binding = visual.Bindings.FirstOrDefault();

                    else
                        binding = visual.Bindings.FirstOrDefault(i => i.Template == Model.Enums.Template.ToastGeneric);

                    if (binding != null)
                    {
                        this.HasContent = true;

                        var container = binding.Container;


                        var texts = container.Children.OfType<AdaptiveTextField>().ToList();
                        List<AdaptiveImage> appLogoOverrides = null;
                        var heroImages = new List<AdaptiveImage>();
                        var attributionTexts = container.Children.OfType<AdaptiveTextField>().Where(i => i.Placement == Model.Enums.TextPlacement.Attribution).ToList();

                        appLogoOverrides = new List<AdaptiveImage>();

                        // First pull out images with placements and attribution text
                        for (Int32 i = 0; i < container.Children.Count; i++)
                        {
                            var child = container.Children[i];

                            if (child is AdaptiveImage)
                            {
                                var img = child as AdaptiveImage;

                                switch (img.Placement)
                                {
                                    case Model.Enums.Placement.AppLogoOverride:
                                        appLogoOverrides.Add(img);
                                        container.RemoveChildAt(i);
                                        i--;
                                        break;

                                    case Model.Enums.Placement.Hero:
                                        heroImages.Add(img);
                                        container.RemoveChildAt(i);
                                        i--;
                                        break;
                                }
                            }

                            else if (child is AdaptiveTextField)
                            {
                                var txt = child as AdaptiveTextField;

                                if (txt.Placement != Model.Enums.TextPlacement.Inline)
                                {
                                    container.RemoveChildAt(i);
                                    i--;
                                }
                            }
                        }

                        // Assign hero
                        if (heroImages.Count != 0)
                        {
                            this.ImageHero.Visibility = Visibility.Visible;
                            this.ImageHeroBrush.ImageSource = ImageHelper.GetBitmap(heroImages.First().Src);
                        }

                        else
                        {
                            this.ImageHero.Visibility = Visibility.Collapsed;
                            this.ImageHeroBrush.ImageSource = null;
                        }


                        texts = [];

                        // Pull out all texts
                        for (Int32 i = 0; i < container.Children.Count; i++)
                        {
                            var child = container.Children[i];

                            if (child is AdaptiveTextField && (child as AdaptiveTextField).Placement == Model.Enums.TextPlacement.Inline)
                            {
                                texts.Add(child as AdaptiveTextField);
                                container.RemoveChildAt(i);
                                i--;
                            }
                        }

                        var titleText = texts.ElementAtOrDefault(0);
                        if (titleText != null)
                        {
                            this.TextBlockTitle.Text = titleText.Text;

                            var bodyTextLine1 = texts.ElementAtOrDefault(1);
                            if (bodyTextLine1 != null)
                            {
                                this.TextBlockBody.Text = bodyTextLine1.Text;

                                var bodyTextLine2 = texts.ElementAtOrDefault(2);
                                if (bodyTextLine2 != null)
                                {
                                    this.TextBlockBody2.Text = bodyTextLine2.Text;
                                    this.TextBlockBody2.Visibility = Visibility.Visible;
                                }

                                this.TextBlockBody.Visibility = Visibility.Visible;
                            }

                            else
                                this.TextBlockBody.Visibility = Visibility.Collapsed;
                        }

                        else
                        {
                            this.TextBlockTitle.Text = this.Properties.DisplayName;
                            this.TextBlockBody.Text = "New notification";
                            this.TextBlockBody.Visibility = Visibility.Visible;
                        }

                        var images = container.Children.OfType<AdaptiveImage>().ToList();


                        if (appLogoOverrides == null)
                            appLogoOverrides = [.. images.Where(i => i.Placement == Model.Enums.Placement.AppLogoOverride)];

                        var appLogoOverride = appLogoOverrides.FirstOrDefault();
                        if (appLogoOverride != null)
                        {
                            this.DefaultImageAppLogo.Visibility = Visibility.Collapsed;

                            var source = ImageHelper.GetBitmap(appLogoOverride.Src);

                            if (appLogoOverride.HintCrop == Model.Enums.HintCrop.Circle)
                            {
                                this.CircleImageAppLogo.Source = source;
                                this.CircleImageAppLogo.Visibility = Visibility.Visible;
                            }

                            else
                            {
                                this.ImageAppLogo.Source = source;
                                this.ImageAppLogo.Visibility = Visibility.Visible;
                            }
                        }

                        foreach (var image in images.Where(i => i.Placement == Model.Enums.Placement.Inline))
                        {
                            var uiImage = new Image()
                            {
                                Source = ImageHelper.GetBitmap(image.Src),
                                MaxHeight = 100,
                                Stretch = Stretch.Uniform,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 6)
                            };

                            this.StackPanelInlineImages.Children.Add(uiImage);
                        }

                        // Attribution
                        if (toastContent.SupportedFeatures.RS1_Style_Toasts)
                        {
                            if (attributionTexts.Count != 0)
                            {
                                this.TextBlockAttributionSeparationDot.Visibility = Visibility.Visible;
                                this.TextBlockAttributionSecondPart.Visibility = Visibility.Visible;
                                this.TextBlockAttributionSecondPart.Text = attributionTexts.First().Text;
                            }

                            else
                            {
                                this.TextBlockAttributionSeparationDot.Visibility = Visibility.Collapsed;
                                this.TextBlockAttributionSecondPart.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }


                if (toastContent.Actions != null)
                {
                    var actions = toastContent.Actions;

                    if (actions.HintSystemCommands == Model.Enums.HintSystemCommands.SnoozeAndDismiss)
                    {
                        //AddInput(CreateComboBox("systemSnoozeSelection", null, new Selection[]
                        //{
                        //    new Selection(NotificationType.Toast, CurrFeatureSet)
                        //    {
                        //        Content = "5 minutes",
                        //        Id = "5"
                        //    },

                        //    new Selection(NotificationType.Toast, CurrFeatureSet)
                        //    {
                        //        Content = "9 minutes",
                        //        Id = "9"
                        //    },

                        //    new Selection(NotificationType.Toast, CurrFeatureSet)
                        //    {
                        //        Content = "10 minutes",
                        //        Id = "10"
                        //    },

                        //    new Selection(NotificationType.Toast, CurrFeatureSet)
                        //    {
                        //        Content = "1 hour",
                        //        Id = "60"
                        //    },

                        //    new Selection(NotificationType.Toast, CurrFeatureSet)
                        //    {
                        //        Content = "4 hours",
                        //        Id = "240"
                        //    },

                        //    new Selection(NotificationType.Toast, CurrFeatureSet)
                        //    {
                        //        Content = "1 day",
                        //        Id = "1440"
                        //    }
                        //}, "9", true));

                        this.AddButton(this.CreateButton("Snooze", "snooze", null, ActivationType.System, toastContent.Scenario));
                        this.AddButton(this.CreateButton("Dismiss", "dismiss", null, ActivationType.System, toastContent.Scenario));
                    }

                    else
                    {
                        var actionElements = actions.ActionElements.ToList();

                        if (toastContent.SupportedFeatures.RS1_Style_Toasts)
                        {
                            actionElements.RemoveAll(i => i.Placement != Model.Enums.ActionPlacement.Inline);
                        }

                        // Initialize inputs
                        foreach (var i in actions.Inputs)
                        {
                            ListViewButton button;

                            switch (i.Type)
                            {
                                case Model.BaseElements.InputType.Text:

                                    button = new TextBoxButton(i);
                                    break;

                                case Model.BaseElements.InputType.Selection:

                                    button = new SelectionButton(i);
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }

                            this.AddButton(button);
                        }

                        // Initialize buttons
                        foreach (var a in actionElements)
                        {
                            ListViewButton uiButton = this.CreateButton(a.Content, a.Arguments, a.ImageUri, a.ActivationType, toastContent.Scenario);

                            //AssignId(uiButton, a.Id);

                            //if (!a.HintVisible)
                            //    uiButton.Visibility = Visibility.Collapsed;

                            this.AddButton(uiButton);
                        }
                    }
                }
            }

            this.ListViewButtons.SelectedIndex = 0;
        }

        internal partial class ListViewButton : BindableBase
        {
            public String Title { get; set; }

            private String _content;
            public String Content
            {
                get { return this._content; }
                set { this.SetProperty(ref this._content, value); }
            }

            public UIElement Icon { get; set; }

            public Action<FrameworkElement> Action { get; set; }

            public override String ToString()
            {
                return this.Content;
            }
        }

        private interface IInput
        {
            Input Input { get; }
            String Value { get; }
        }

        internal partial class TextBoxButton : ListViewButton, IInput
        {
            public Input Input { get; private set; }

            public TextBoxButton(Input input)
            {
                this.Input = input;

                // Automatically assigns Content too
                this.Value = input.DefaultInput;

                this.Title = input.Title;

                this.Icon = new SymbolIcon(Symbol.Keyboard);

                this.Action = this.ShowPopup;
            }

            private String _value;
            /// <summary>
            /// The actual text value the user typed. Content will reflect this
            /// </summary>
            public String Value
            {
                get { return this._value; }
                private set
                {
                    this._value = value;
                    if (value == null || value.Length == 0)
                    {
                        this.Content = this.Input.PlaceHolderContent;
                        this._value = String.Empty;
                    }
                    else
                    {
                        this.Content = value;
                    }
                }
            }

            private void ShowPopup(FrameworkElement el)
            {
                var textBox = new TextBox()
                {
                    Header = this.Input.Title,
                    Text = this.Value
                };

                var buttonEnter = new Button()
                {
                    Content = "Enter",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Microsoft.UI.Xaml.Thickness(0, 12, 0, 0)
                };

                var flyout = new Flyout {
                    Content = new StackPanel() {
                        Width = 300,
                        Children =
                        {
                            textBox,
                            buttonEnter
                        }
                    }
                };

                System.Action actionEnter = delegate
                {
                    this.Value = textBox.Text;
                    flyout.Hide();
                };

                buttonEnter.Click += delegate
                {
                    actionEnter();
                };

                textBox.KeyUp += (s, e) =>
                {
                    if (e.Key == Windows.System.VirtualKey.Enter)
                    {
                        e.Handled = true;
                        actionEnter();
                    }
                };

                flyout.ShowAt(el);
            }
        }

        internal partial class SelectionButton : ListViewButton, IInput
        {
            public Input Input { get; private set; }

            public SelectionButton(Input input)
            {
                this.Input = input;

                if (input.DefaultInput != null)
                {
                    var defaultSelection = input.Children.FirstOrDefault(i => i.Id == input.DefaultInput);
                    this.SelectItem(defaultSelection);
                }

                this.Title = input.Title;

                this.Icon = new Viewbox()
                {
                    Child = new SymbolIcon(Symbol.Edit),
                    Height = 15,
                    Width = 15
                };

                this.Action = this.ShowPopup;
            }

            private void SelectItem(Selection selection)
            {
                this.Value = selection.Id;
                this.Content = selection.Content;
            }

            private String _value;
            /// <summary>
            /// The actual text value the user typed. Content will reflect this
            /// </summary>
            public String Value
            {
                get { return this._value; }
                private set
                {
                    this._value = value;
                    if (value == null || value.Length == 0)
                    {
                        this.Content = this.Input.PlaceHolderContent;
                        this._value = String.Empty;
                    }
                    else
                    {
                        this.Content = value;
                    }
                }
            }

            private void ShowPopup(FrameworkElement el)
            {
                var listView = new ListView()
                {
                    Header = this.Input.Title,
                    DisplayMemberPath = "Content",
                    ItemsSource = this.Input.Children,
                    SelectedItem = this.Input.Children.FirstOrDefault(i => i.Id == this.Value),
                    IsItemClickEnabled = true
                };

                var flyout = new Flyout {
                    Content = new StackPanel() {
                        Width = 300,
                        Children =
                        {
                            listView
                        }
                    }
                };

                listView.ItemClick += (s, e) =>
                {
                    this.SelectItem(e.ClickedItem as Selection);
                    flyout.Hide();
                };

                flyout.ShowAt(el);
            }
        }

        private ListViewButton CreateButton(String content, String arguments, String imageUri, ActivationType activationType, Scenario scenarioType)
        {
            // Auto-generate content
            if (activationType == ActivationType.System && content.Length == 0)
            {
                // Case-sensitive
                switch (arguments)
                {
                    case "snooze":
                        content = "Snooze";
                        break;

                    case "dismiss":
                        content = "Dismiss";
                        break;
                }
            }

            var b = new ListViewButton {
                Content = content,
                Action = delegate {
                    this.TriggerActivation(arguments, activationType);
                }
            };

            return b;
        }

        private void AddButton(ListViewButton el)
        {
            if (this._buttons.Count == 0)
            {
                this._buttons.Add(el);
            }
            else
            {
                this._buttons.Insert(this._buttons.Count - 1, el);
            }
        }

        private void TriggerActivation(String arguments, ActivationType activationType)
        {
            if (arguments == null)
                arguments = "";

            var args = new PreviewToastNotificationActivatedEventArgs(arguments, this.GetCurrentUserInput());

            switch (activationType)
            {
                case ActivationType.Background:
                    if (ActivatedBackground != null)
                        ActivatedBackground(this, args);
                    break;

                case ActivationType.Foreground:
                    if (ActivatedForeground != null)
                        ActivatedForeground(this, args);
                    break;

                case ActivationType.Protocol:
                    if (ActivatedProtocol != null)
                        ActivatedProtocol(this, args);
                    break;

                case ActivationType.System:
                    if (ActivatedSystem != null)
                        ActivatedSystem(this, args);
                    break;
            }
        }

        private ValueSet GetCurrentUserInput()
        {
            var answer = new ValueSet();

            //foreach (var input in _currInputs)
            //{
            //    answer.Add(input.Id, input.Value);
            //}

            return answer;
        }

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewToastProperties), typeof(PreviewXboxToastInActionCenter), new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get { return this.GetValue(PropertiesProperty) as PreviewToastProperties; }
            set { this.SetValue(PropertiesProperty, value); }
        }

        #region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewXboxToastInActionCenter), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

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
            (sender as PreviewXboxToastInActionCenter).OnDeviceFamilyChanged(e);
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
            (sender as PreviewXboxToastInActionCenter).OnOSBuildNumberChanged(e);
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

        private void ListViewButtons_ItemClick(Object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ListViewButton;
            if (item != null && item.Action != null)
            {
                item.Action(e.OriginalSource as FrameworkElement);
            }
        }
    }
}
