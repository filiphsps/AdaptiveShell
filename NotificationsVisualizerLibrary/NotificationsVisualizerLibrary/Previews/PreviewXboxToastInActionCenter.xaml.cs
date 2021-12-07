using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class PreviewXboxToastInActionCenter : UserControl, IPreviewToast
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();

        private ObservableCollection<ListViewButton> _buttons = new ObservableCollection<ListViewButton>();

        public event EventHandler<PreviewToastNotificationActivatedEventArgs> ActivatedForeground, ActivatedBackground, ActivatedSystem, ActivatedProtocol;

        public PreviewXboxToastInActionCenter()
        {
            this.InitializeComponent();

            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            ListViewButtons.ItemsSource = _buttons;
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
            _currContent = toastContent;
            HasContent = false;

            TextBlockTitle.Text = "";
            TextBlockTitle.MaxLines = 3;
            TextBlockTitle.TextWrapping = TextWrapping.WrapWholeWords;

            TextBlockBody.Text = "";
            TextBlockBody.MaxLines = 3;
            TextBlockBody.TextWrapping = TextWrapping.WrapWholeWords;

            TextBlockBody2.Visibility = Visibility.Collapsed;
            TextBlockBody2.MaxLines = 3;
            TextBlockBody2.TextWrapping = TextWrapping.WrapWholeWords;

            StackPanelInlineImages.Children.Clear();

            DefaultImageAppLogo.Visibility = Visibility.Visible;
            ImageAppLogo.Visibility = Visibility.Collapsed;
            CircleImageAppLogo.Visibility = Visibility.Collapsed;

            _elementsWithIds = new Dictionary<string, FrameworkElement>();

            _currLaunch = "";
            _currActivationType = ActivationType.Foreground;

            _buttons.Clear();
            _buttons.Add(CreateButton("Launch", toastContent?.Launch, null, toastContent != null ? toastContent.ActivationType.GetValueOrDefault(ActivationType.Foreground) : ActivationType.Foreground, Scenario.Default));



            if (toastContent != null)
            {
                _currLaunch = toastContent.Launch;
                _currActivationType = toastContent.ActivationType.GetValueOrDefault(ActivationType.Foreground);

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
                        HasContent = true;

                        var container = binding.Container;


                        var texts = container.Children.OfType<AdaptiveTextField>().ToList();
                        List<AdaptiveImage> appLogoOverrides = null;
                        List<AdaptiveImage> heroImages = new List<AdaptiveImage>();
                        var attributionTexts = container.Children.OfType<AdaptiveTextField>().Where(i => i.Placement == Model.Enums.TextPlacement.Attribution).ToList();

                        appLogoOverrides = new List<AdaptiveImage>();

                        // First pull out images with placements and attribution text
                        for (int i = 0; i < container.Children.Count; i++)
                        {
                            var child = container.Children[i];

                            if (child is AdaptiveImage)
                            {
                                AdaptiveImage img = child as AdaptiveImage;

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
                                AdaptiveTextField txt = child as AdaptiveTextField;

                                if (txt.Placement != Model.Enums.TextPlacement.Inline)
                                {
                                    container.RemoveChildAt(i);
                                    i--;
                                }
                            }
                        }

                        // Assign hero
                        if (heroImages.Any())
                        {
                            ImageHero.Visibility = Visibility.Visible;
                            ImageHeroBrush.ImageSource = ImageHelper.GetBitmap(heroImages.First().Src);
                        }

                        else
                        {
                            ImageHero.Visibility = Visibility.Collapsed;
                            ImageHeroBrush.ImageSource = null;
                        }


                        texts = new List<AdaptiveTextField>();

                        // Pull out all texts
                        for (int i = 0; i < container.Children.Count; i++)
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
                            TextBlockTitle.Text = titleText.Text;

                            var bodyTextLine1 = texts.ElementAtOrDefault(1);
                            if (bodyTextLine1 != null)
                            {
                                TextBlockBody.Text = bodyTextLine1.Text;

                                var bodyTextLine2 = texts.ElementAtOrDefault(2);
                                if (bodyTextLine2 != null)
                                {
                                    TextBlockBody2.Text = bodyTextLine2.Text;
                                    TextBlockBody2.Visibility = Visibility.Visible;
                                }

                                TextBlockBody.Visibility = Visibility.Visible;
                            }

                            else
                                TextBlockBody.Visibility = Visibility.Collapsed;
                        }

                        else
                        {
                            TextBlockTitle.Text = Properties.DisplayName;
                            TextBlockBody.Text = "New notification";
                            TextBlockBody.Visibility = Visibility.Visible;
                        }

                        var images = container.Children.OfType<AdaptiveImage>().ToList();


                        if (appLogoOverrides == null)
                            appLogoOverrides = images.Where(i => i.Placement == Model.Enums.Placement.AppLogoOverride).ToList();

                        var appLogoOverride = appLogoOverrides.FirstOrDefault();
                        if (appLogoOverride != null)
                        {
                            DefaultImageAppLogo.Visibility = Visibility.Collapsed;

                            var source = ImageHelper.GetBitmap(appLogoOverride.Src);

                            if (appLogoOverride.HintCrop == Model.Enums.HintCrop.Circle)
                            {
                                CircleImageAppLogo.Source = source;
                                CircleImageAppLogo.Visibility = Visibility.Visible;
                            }

                            else
                            {
                                ImageAppLogo.Source = source;
                                ImageAppLogo.Visibility = Visibility.Visible;
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
                                Margin = new Thickness(0, 0, 0, 6)
                            };

                            StackPanelInlineImages.Children.Add(uiImage);
                        }

                        // Attribution
                        if (toastContent.SupportedFeatures.RS1_Style_Toasts)
                        {
                            if (attributionTexts.Any())
                            {
                                TextBlockAttributionSeparationDot.Visibility = Visibility.Visible;
                                TextBlockAttributionSecondPart.Visibility = Visibility.Visible;
                                TextBlockAttributionSecondPart.Text = attributionTexts.First().Text;
                            }

                            else
                            {
                                TextBlockAttributionSeparationDot.Visibility = Visibility.Collapsed;
                                TextBlockAttributionSecondPart.Visibility = Visibility.Collapsed;
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

                        AddButton(CreateButton("Snooze", "snooze", null, ActivationType.System, toastContent.Scenario));
                        AddButton(CreateButton("Dismiss", "dismiss", null, ActivationType.System, toastContent.Scenario));
                    }

                    else
                    {
                        List<Model.BaseElements.Action> actionElements = actions.ActionElements.ToList();

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

                            AddButton(button);
                        }

                        // Initialize buttons
                        foreach (var a in actionElements)
                        {
                            ListViewButton uiButton = CreateButton(a.Content, a.Arguments, a.ImageUri, a.ActivationType, toastContent.Scenario);

                            //AssignId(uiButton, a.Id);

                            //if (!a.HintVisible)
                            //    uiButton.Visibility = Visibility.Collapsed;

                            AddButton(uiButton);
                        }
                    }
                }
            }

            ListViewButtons.SelectedIndex = 0;
        }

        internal class ListViewButton : BindableBase
        {
            public string Title { get; set; }

            private string _content;
            public string Content
            {
                get { return _content; }
                set { SetProperty(ref _content, value); }
            }

            public UIElement Icon { get; set; }

            public Action<FrameworkElement> Action { get; set; }

            public override string ToString()
            {
                return Content;
            }
        }

        private interface IInput
        {
            Input Input { get; }
            string Value { get; }
        }

        internal class TextBoxButton : ListViewButton, IInput
        {
            public Input Input { get; private set; }

            public TextBoxButton(Input input)
            {
                Input = input;

                // Automatically assigns Content too
                Value = input.DefaultInput;

                Title = input.Title;

                Icon = new SymbolIcon(Symbol.Keyboard);

                Action = ShowPopup;
            }

            private string _value;
            /// <summary>
            /// The actual text value the user typed. Content will reflect this
            /// </summary>
            public string Value
            {
                get { return _value; }
                private set
                {
                    _value = value;
                    if (value == null || value.Length == 0)
                    {
                        Content = Input.PlaceHolderContent;
                        _value = string.Empty;
                    }
                    else
                    {
                        Content = value;
                    }
                }
            }

            private void ShowPopup(FrameworkElement el)
            {
                TextBox textBox = new TextBox()
                {
                    Header = Input.Title,
                    Text = Value
                };

                Button buttonEnter = new Button()
                {
                    Content = "Enter",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(0, 12, 0, 0)
                };

                Flyout flyout = new Flyout();
                flyout.Content = new StackPanel()
                {
                    Width = 300,
                    Children =
                    {
                        textBox,
                        buttonEnter
                    }
                };

                System.Action actionEnter = delegate
                {
                    Value = textBox.Text;
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

        internal class SelectionButton : ListViewButton, IInput
        {
            public Input Input { get; private set; }

            public SelectionButton(Input input)
            {
                Input = input;

                if (input.DefaultInput != null)
                {
                    var defaultSelection = input.Children.FirstOrDefault(i => i.Id == input.DefaultInput);
                    SelectItem(defaultSelection);
                }

                Title = input.Title;

                Icon = new Viewbox()
                {
                    Child = new SymbolIcon(Symbol.Edit),
                    Height = 15,
                    Width = 15
                };

                Action = ShowPopup;
            }

            private void SelectItem(Selection selection)
            {
                Value = selection.Id;
                Content = selection.Content;
            }

            private string _value;
            /// <summary>
            /// The actual text value the user typed. Content will reflect this
            /// </summary>
            public string Value
            {
                get { return _value; }
                private set
                {
                    _value = value;
                    if (value == null || value.Length == 0)
                    {
                        Content = Input.PlaceHolderContent;
                        _value = string.Empty;
                    }
                    else
                    {
                        Content = value;
                    }
                }
            }

            private void ShowPopup(FrameworkElement el)
            {
                ListView listView = new ListView()
                {
                    Header = Input.Title,
                    DisplayMemberPath = "Content",
                    ItemsSource = Input.Children,
                    SelectedItem = Input.Children.FirstOrDefault(i => i.Id == Value),
                    IsItemClickEnabled = true
                };

                Flyout flyout = new Flyout();
                flyout.Content = new StackPanel()
                {
                    Width = 300,
                    Children =
                    {
                        listView
                    }
                };

                listView.ItemClick += (s, e) =>
                {
                    SelectItem(e.ClickedItem as Selection);
                    flyout.Hide();
                };

                flyout.ShowAt(el);
            }
        }

        private ListViewButton CreateButton(string content, string arguments, string imageUri, ActivationType activationType, Scenario scenarioType)
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

            ListViewButton b = new ListViewButton()
            {
                Content = content
            };

            b.Action = delegate
            {
                TriggerActivation(arguments, activationType);
            };

            return b;
        }

        private void AddButton(ListViewButton el)
        {
            if (_buttons.Count == 0)
            {
                _buttons.Add(el);
            }
            else
            {
                _buttons.Insert(_buttons.Count - 1, el);
            }
        }

        private void TriggerActivation(string arguments, ActivationType activationType)
        {
            if (arguments == null)
                arguments = "";

            PreviewToastNotificationActivatedEventArgs args = new PreviewToastNotificationActivatedEventArgs(arguments, GetCurrentUserInput());

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
            ValueSet answer = new ValueSet();

            //foreach (var input in _currInputs)
            //{
            //    answer.Add(input.Id, input.Value);
            //}

            return answer;
        }

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewToastProperties), typeof(PreviewXboxToastInActionCenter), new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get { return GetValue(PropertiesProperty) as PreviewToastProperties; }
            set { SetValue(PropertiesProperty, value); }
        }

        #region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewXboxToastInActionCenter), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

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
            (sender as PreviewXboxToastInActionCenter).OnDeviceFamilyChanged(e);
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
            (sender as PreviewXboxToastInActionCenter).OnOSBuildNumberChanged(e);
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

        private void ListViewButtons_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ListViewButton;
            if (item != null && item.Action != null)
            {
                item.Action(e.OriginalSource as FrameworkElement);
            }
        }
    }
}
