using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Dynamic;
using System.Diagnostics;
using Windows.Foundation.Metadata;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public sealed class PreviewToastProperties
    {
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 120, 215);

        public Uri Square44x44Logo { get; set; }

        public string DisplayName { get; set; }
    }

    public sealed class PreviewToastNotificationActivatedEventArgs
    {
        internal PreviewToastNotificationActivatedEventArgs(string argument, ValueSet userInput)
        {
            Argument = argument;
            UserInput = userInput;
        }

        public string Argument { get; private set; }

        public ValueSet UserInput { get; private set; }
    }

    public sealed partial class PreviewToast : UserControl, IPreviewToast
    {
        public static event EventHandler<Exception> OnRenderingErrorOccurred;
        internal static void SendRenderingError(Exception ex)
        {
            OnRenderingErrorOccurred?.Invoke(null, ex);
        }

        private static XmlTemplateParser _parser = new XmlTemplateParser();

        public event EventHandler<PreviewToastNotificationActivatedEventArgs> ActivatedForeground, ActivatedBackground, ActivatedSystem, ActivatedProtocol;

        public PreviewToast()
        {
            this.InitializeComponent();

            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            TextBlockAttributionFirstPart.SetBinding(TextBlock.TextProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Properties.DisplayName")
            });

            AppName.SetBinding(TextBlock.TextProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Properties.DisplayName")
            });

            //if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
            //{
            //    MainContainer.Background = new AcrylicBrush()
            //    {
            //        TintColor = Color.FromArgb(255, 31, 31, 31),
            //        Opacity = 0.8,
            //        BackgroundSource = AcrylicBackgroundSource.HostBackdrop,
            //        FallbackColor = Color.FromArgb(255, 31, 31, 31)
            //    };
            //}
            AssignMenuFlyout(null);
        }

        private void AssignMenuFlyout(MenuFlyoutItem[] firstItems)
        {
            try
            {
                // ContextFlyout was added in Anniversary Update
                if (ApiInformation.IsPropertyPresent(typeof(UIElement).FullName, nameof(UIElement.ContextFlyout)))
                {
                    var menu = new MenuFlyout();

                    if (firstItems != null && firstItems.Length > 0)
                    {
                        foreach (var item in firstItems)
                        {
                            menu.Items.Add(item);
                        }

                        menu.Items.Add(new MenuFlyoutSeparator());
                    }

                    menu.Items.Add(new MenuFlyoutItem()
                    {
                        Text = "Go to notification settings"
                    });

                    menu.Items.Add(new MenuFlyoutItem()
                    {
                        Text = "Turn off notifications for " + Properties?.DisplayName
                    });

                    this.ContextFlyout = menu;
                }
            }
            catch
            {
                // In the future, I should have a callback where parent client can obtain and report exceptions
            }
        }

        private List<IInput> _currInputs = new List<IInput>();

        /// <summary>
        /// Call this to initialize the toast with new toast content. This can be called multiple times, and will clear the previous content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ParseResult Initialize(XmlDocument content)
        {
            return Initialize(content, new PreviewNotificationData());
        }

        private DataBindingValues _lastDataBindingValues;
        /// <summary>
        /// Call this to initialize the toast with new toast content and data. This can be called multiple times, and will clear the previous content.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Call this to update the toast's data.
        /// </summary>
        /// <param name="data"></param>
        public void Update(PreviewNotificationData data)
        {
            if (_lastDataBindingValues == null || !(_currContent is Toast) || data.Version < _lastDataBindingValues.Version)
            {
                return;
            }
            _lastDataBindingValues.Update(data.Values);
            (_currContent as Toast).ApplyDataBinding(_lastDataBindingValues);
        }

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewToastProperties), typeof(PreviewToast), new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get { return GetValue(PropertiesProperty) as PreviewToastProperties; }
            set { SetValue(PropertiesProperty, value); }
        }

        private static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(PreviewToast), new PropertyMetadata(true, OnIsExpandedChanged));

        private static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewToast).OnIsExpandedChanged(e);
        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsExpanded)
            {
                if (CurrFeatureSet != null && CurrFeatureSet.RS1_Style_Toasts)
                    VisualStateManager.GoToState(this, "ExpandedAdaptiveState", false);
                else
                    VisualStateManager.GoToState(this, "ExpandedState", false);
            }

            else
            {
                if (CurrFeatureSet != null && CurrFeatureSet.RS1_Style_Toasts)
                    VisualStateManager.GoToState(this, "CollapsedStateRS1", false);
                else
                    VisualStateManager.GoToState(this, "CollapsedState", false);
            }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        private interface IInput
        {
            string Id { get; }
            string Value { get; }
        }

        private class TextInput : IInput
        {
            private TextBox _tb;
            public TextInput(TextBox tb)
            {
                _tb = tb;
            }

            public string Id
            {
                get
                {
                    return (string)_tb.Tag;
                }
            }

            public string Value
            {
                get
                {
                    return _tb.Text;
                }
            }
        }

        private class SelectionInput : IInput
        {
            private ComboBox _cb;

            public SelectionInput(ComboBox cb)
            {
                _cb = cb;
            }

            public string Id
            {
                get
                {
                    return (string)_cb.Tag;
                }
            }

            public string Value
            {
                get
                {
                    var selection = _cb.SelectedItem as Selection;

                    if (selection != null)
                        return selection.Id;

                    return "";
                }
            }
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

        private void ResetContent()
        {
            HasContent = false;

            TextBlockTitle.Text = "";
            TextBlockTitle.MaxLines = 2;
            TextBlockTitle.TextWrapping = TextWrapping.WrapWholeWords;

            // In RS3+, title text is SemiBold (Base style)
            // In RS1-RS2, title text isn't bold (Body style)
            // In Pre-RS1, title text was SemiBold
            if (CurrFeatureSet.RS3_Style_Toasts)
            {
                TextBlockTitle.Style = (Style)Resources["BaseTextBlockStyle"];
            }
            else if (CurrFeatureSet.RS1_Style_Toasts)
            {
                TextBlockTitle.Style = (Style)Resources["BodyTextBlockStyle"];
            }
            else
            {
                TextBlockTitle.Style = (Style)Resources["BaseTextBlockStyle"];
            }

            TextBlockBody.Text = "";
            TextBlockBody.MaxLines = 4;
            TextBlockBody.TextWrapping = TextWrapping.WrapWholeWords;

            TextBlockHeaderWhenAppLogoNotPresent.Text = "";
            TextBlockHeaderWhenAppLogoPresent.Text = "";

            ContentAdaptive.Child = null;

            ButtonsContainer.Items.Clear();

            StackPanelInlineImages.Children.Clear();
            StackPanelInputs.Children.Clear();

            CallingImageContainer.Child = null;

            Pre19H2AppLogo.Visibility = CurrFeatureSet.R_19H2_Style_Toasts ? Visibility.Collapsed : Visibility.Visible;
            ImageAppLogo.Visibility = Visibility.Collapsed;
            CircleImageAppLogo.Visibility = Visibility.Collapsed;

            _elementsWithIds = new Dictionary<string, FrameworkElement>();

            _currInputs.Clear();
            _currLaunch = "";
            _currActivationType = ActivationType.Foreground;

            AssignMenuFlyout(null);

            StackPanelAttribution.Visibility = Visibility.Collapsed;
            TextBlockAttributionFirstPart.Visibility = Visibility.Collapsed;
            TextBlockAttributionSeparationDot.Visibility = Visibility.Collapsed;
            TextBlockAttributionSecondPart.Visibility = Visibility.Collapsed;
            AppLogoOverrideContainer.Visibility = Visibility.Collapsed;
        }

        private void InitializeContent(IToast toastContent)
        {
            ResetContent();

            _currContent = toastContent;

            if (toastContent != null)
            {
                _currLaunch = toastContent.Launch;
                _currActivationType = toastContent.ActivationType.GetValueOrDefault(ActivationType.Foreground);

                switch (toastContent.Scenario)
                {
                    case Scenario.IncomingCall:
                        ButtonsContainer.ItemsPanel = (ItemsPanelTemplate)Resources["CallingButtonsPanelTemplate"];
                        ButtonsContainer.Margin = new Thickness();
                        break;

                    default:
                        ButtonsContainer.ItemsPanel = (ItemsPanelTemplate)Resources["NormalButtonsPanelTemplate"];
                        ButtonsContainer.Margin = CurrFeatureSet.RS3_Style_Toasts ? new Thickness(16, 0, 16, 0) : new Thickness(12, 0, 12, 0);
                        break;
                }

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
                        var children = new List<AdaptiveChildElement>(container.Children);


                        var texts = children.OfType<AdaptiveTextField>().ToList();
                        List<AdaptiveImage> appLogoOverrides = null;
                        List<AdaptiveImage> heroImages = new List<AdaptiveImage>();
                        var attributionTexts = children.OfType<AdaptiveTextField>().Where(i => i.Placement == Model.Enums.TextPlacement.Attribution).ToList();


                        if (CurrFeatureSet.AdaptiveToasts)
                        {
                            appLogoOverrides = new List<AdaptiveImage>();

                            // First pull out images with placements and attribution text
                            for (int i = 0; i < children.Count; i++)
                            {
                                var child = children[i];

                                if (child is AdaptiveImage)
                                {
                                    AdaptiveImage img = child as AdaptiveImage;

                                    switch (img.Placement)
                                    {
                                        case Model.Enums.Placement.AppLogoOverride:
                                            appLogoOverrides.Add(img);
                                            children.RemoveAt(i);
                                            i--;
                                            break;

                                        case Model.Enums.Placement.Hero:
                                            heroImages.Add(img);
                                            children.RemoveAt(i);
                                            i--;
                                            break;
                                    }
                                }

                                else if (child is AdaptiveTextField)
                                {
                                    AdaptiveTextField txt = child as AdaptiveTextField;

                                    if (txt.Placement != Model.Enums.TextPlacement.Inline)
                                    {
                                        children.RemoveAt(i);
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
                            for (int i = 0; i < children.Count; i++)
                            {
                                var child = children[i];

                                if (child is AdaptiveTextField && (child as AdaptiveTextField).Placement == Model.Enums.TextPlacement.Inline)
                                {
                                    texts.Add(child as AdaptiveTextField);
                                    children.RemoveAt(i);
                                    i--;
                                }
                            }
                        }


                        var titleText = texts.ElementAtOrDefault(0);
                        if (titleText != null)
                        {
                            TextBlockTitle.Text = titleText.Text;

                            if (CurrFeatureSet.AdaptiveToasts)
                            {
                                TextBlockTitle.MaxLines = Math.Min(titleText.HintMaxLines.GetValueOrDefault(2), 2);
                                TextBlockTitle.TextWrapping = titleText.HintWrap.GetValueOrDefault(true) ? TextWrapping.WrapWholeWords : TextWrapping.NoWrap;
                            }

                            var bodyTextLine1 = texts.ElementAtOrDefault(1);
                            if (bodyTextLine1 != null)
                            {
                                if (CurrFeatureSet.AdaptiveToasts)
                                {
                                    TextBlockBody.Text = bodyTextLine1.Text;
                                    TextBlockBody.MaxLines = Math.Min(bodyTextLine1.HintMaxLines.GetValueOrDefault(4), 4);

                                    var bodyTextLine2 = texts.ElementAtOrDefault(2);
                                    if (bodyTextLine2 != null)
                                    {
                                        TextBlockBody.Text += "\n" + bodyTextLine2.Text;
                                    }
                                }

                                else
                                {
                                    var bodyTextLine2 = texts.ElementAtOrDefault(2);
                                    if (bodyTextLine2 != null)
                                        TextBlockBody.Text = bodyTextLine1.Text + "\n" + bodyTextLine2.Text;
                                    else
                                        TextBlockBody.Text = bodyTextLine1.Text;
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

                        var images = children.OfType<AdaptiveImage>().ToList();

                        if (toastContent.Scenario == Scenario.IncomingCall)
                        {
                            var callingImage = images.FirstOrDefault(i => i.Placement == Model.Enums.Placement.Inline);
                            if (callingImage != null)
                            {
                                images.Remove(callingImage);

                                switch (callingImage.HintCrop)
                                {
                                    case Model.Enums.HintCrop.Circle:
                                        CallingImageContainer.Child = AssignId(new CircleImage()
                                        {
                                            Source = ImageHelper.GetBitmap(callingImage.Src),
                                            Width = 96,
                                            Height = 96,
                                            Margin = new Thickness(0, 0, 0, 12)
                                        }, callingImage.Id);
                                        break;

                                    default:
                                        CallingImageContainer.Child = AssignId(new Image()
                                        {
                                            Source = ImageHelper.GetBitmap(callingImage.Src),
                                            Width = 200,
                                            Height = 200,
                                            Stretch = Stretch.Uniform,
                                            Margin = new Thickness(0, 0, 0, 12)
                                        }, callingImage.Id);
                                        break;
                                }
                            }
                        }


                        if (appLogoOverrides == null)
                            appLogoOverrides = images.Where(i => i.Placement == Model.Enums.Placement.AppLogoOverride).ToList();

                        var appLogoOverride = appLogoOverrides.FirstOrDefault();
                        if (appLogoOverride != null)
                        {
                            Pre19H2AppLogo.Visibility = Visibility.Collapsed;
                            AppLogoOverrideContainer.Visibility = Visibility.Visible;

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

                            if (!CurrFeatureSet.R_19H2_Style_Toasts && CurrFeatureSet.RS1_Style_Toasts)
                            {
                                // We also show the attribution app name
                                StackPanelAttribution.Visibility = Visibility.Visible;
                                TextBlockAttributionFirstPart.Visibility = Visibility.Visible;
                            }
                        }

                        if (toastContent.Header != null)
                        {
                            if (appLogoOverride != null)
                            {
                                TextBlockHeaderWhenAppLogoPresent.Text = toastContent.Header.Title;
                            }
                            else
                            {
                                TextBlockHeaderWhenAppLogoNotPresent.Text = toastContent.Header.Title;
                            }
                        }

                        foreach (var image in images.Where(i => i.Placement == Model.Enums.Placement.Inline))
                        {
                            var uiImage = new Image()
                            {
                                Source = ImageHelper.GetBitmap(image.Src),
                                MaxWidth = 84,
                                MaxHeight = 84,
                                Stretch = Stretch.Uniform,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Margin = new Thickness(0, 12, 0, 0)
                            };

                            StackPanelInlineImages.Children.Add(uiImage);
                        }


                        // Initialize adaptive content
                        if (toastContent.SupportedFeatures.AdaptiveToasts && toastContent.Scenario != Scenario.IncomingCall)
                        {
                            if (children.Count > 0)
                            {
                                // Move any progress bars to the top
                                if (children.OfType<AdaptiveProgress>().Any())
                                {
                                    int insertProgressIndex = 0;
                                    for (int i = 0; i < children.Count; i++)
                                    {
                                        var curr = children[i];

                                        // If curr is adaptive progress
                                        if (curr is AdaptiveProgress)
                                        {
                                            // If it should be moved
                                            if (insertProgressIndex != i)
                                            {
                                                // Move progress to front
                                                children.RemoveAt(i);
                                                children.Insert(insertProgressIndex, curr);
                                            }

                                            // Increment so next insert goes after this
                                            insertProgressIndex++;
                                        }
                                    }
                                }

                                var oldChildren = binding.Container.Children.ToArray();
                                binding.Container.SwapChildren(children);

                                PreviewTileNotification notif = new PreviewTileNotification()
                                {
                                };
                                notif.InitializeFromXml(TileSize.Large, new PreviewTileVisualElements()
                                {
                                    BackgroundColor = Colors.Transparent
                                }, isBrandingVisible: false,
                                binding: binding);

                                binding.Container.SwapChildren(oldChildren);

                                ContentAdaptive.Child = notif;
                            }
                        }



                        // Attribution
                        if (toastContent.SupportedFeatures.RS1_Style_Toasts)
                        {
                            if (attributionTexts.Any())
                            {
                                StackPanelAttribution.Visibility = Visibility.Visible;

                                if (TextBlockAttributionFirstPart.Visibility == Visibility.Visible)
                                {
                                    TextBlockAttributionSeparationDot.Visibility = Visibility.Visible;
                                }

                                TextBlockAttributionSecondPart.Visibility = Visibility.Visible;
                                TextBlockAttributionSecondPart.Text = attributionTexts.First().Text;
                            }
                        }
                    }
                }


                if (toastContent.Actions != null)
                {
                    var actions = toastContent.Actions;

                    if (actions.HintSystemCommands == Model.Enums.HintSystemCommands.SnoozeAndDismiss)
                    {
                        AddInput(CreateComboBox("systemSnoozeSelection", null, new Selection[]
                        {
                            new Selection(NotificationType.Toast, CurrFeatureSet)
                            {
                                Content = "5 minutes",
                                Id = "5"
                            },

                            new Selection(NotificationType.Toast, CurrFeatureSet)
                            {
                                Content = "9 minutes",
                                Id = "9"
                            },

                            new Selection(NotificationType.Toast, CurrFeatureSet)
                            {
                                Content = "10 minutes",
                                Id = "10"
                            },

                            new Selection(NotificationType.Toast, CurrFeatureSet)
                            {
                                Content = "1 hour",
                                Id = "60"
                            },

                            new Selection(NotificationType.Toast, CurrFeatureSet)
                            {
                                Content = "4 hours",
                                Id = "240"
                            },

                            new Selection(NotificationType.Toast, CurrFeatureSet)
                            {
                                Content = "1 day",
                                Id = "1440"
                            }
                        }, "9", true));

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
                            // See if we can pull out a related action to display on side
                            Model.BaseElements.Action a = null;

                            // We only place ajacent to text boxes
                            if (i.Type == InputType.Text)
                                a = actionElements.FirstOrDefault(x => object.Equals(x.InputId, i.Id));

                            if (a != null)
                                actionElements.Remove(a);

                            Grid grid = new Grid();

                            if (a != null)
                            {
                                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                                var uiButton = CreateImageButton(a.ImageUri, a.Arguments, a.ActivationType);

                                AssignId(uiButton, a.Id);

                                if (!a.HintVisible)
                                    uiButton.Visibility = Visibility.Collapsed;

                                Grid.SetColumn(uiButton, 1);
                                grid.Children.Add(uiButton);
                            }

                            switch (i.Type)
                            {
                                case Model.BaseElements.InputType.Text:

                                    TextBox tb = CreateTextBox(
                                        id: i.Id,
                                        title: i.Title,
                                        defaultInput: i.DefaultInput,
                                        placeHolderContent: i.PlaceHolderContent,
                                        hintVisible: i.HintVisible);

                                    grid.Children.Add(tb);
                                    break;

                                case Model.BaseElements.InputType.Selection:

                                    ComboBox cb = CreateComboBox(i.Id, i.Title, i.Children.OfType<Selection>().ToArray(), i.DefaultInput, i.HintVisible);

                                    grid.Children.Add(cb);

                                    break;

                                default:
                                    throw new NotImplementedException();
                            }

                            AddInput(grid);
                        }

                        // If there's only one button
                        if (actionElements.Count == 1 && toastContent.Scenario != Scenario.IncomingCall)
                        {
                            Model.BaseElements.Action a = actionElements[0];

                            Button uiButton = CreateButton(a.Content, a.Arguments, a.ImageUri, a.ActivationType, toastContent.Scenario);

                            AssignId(uiButton, a.Id);

                            if (!a.HintVisible)
                                uiButton.Visibility = Visibility.Collapsed;

                            if (CurrFeatureSet.R_19H1_Style_Toasts)
                            {
                                // In 19H1 (and possibly even earlier), single buttons stretch full width...
                                // so do nothing at all!
                            }

                            else if (CurrFeatureSet.RS3_Style_Toasts
                                || !CurrFeatureSet.RS1_Style_Toasts)
                            {
                                // In RS3 and higher, and also before RS1, we have it consume half the width on the right
                                // by adding a blank entry first
                                AddButton(new Border());
                            }

                            else
                            {
                                // In Anniversary Update the button will increase to fit the text
                                uiButton.HorizontalAlignment = HorizontalAlignment.Right;
                            }

                            // Then add the actual button (so it appears on the right half)
                            AddButton(uiButton);
                        }

                        else
                        {
                            // Initialize buttons
                            foreach (var a in actionElements)
                            {
                                Button uiButton = CreateButton(a.Content, a.Arguments, a.ImageUri, a.ActivationType, toastContent.Scenario);

                                AssignId(uiButton, a.Id);

                                if (!a.HintVisible)
                                    uiButton.Visibility = Visibility.Collapsed;

                                AddButton(uiButton);
                            }
                        }
                    }

                    if (actions.ContextMenuItems != null && actions.ContextMenuItems.Any())
                    {
                        List<MenuFlyoutItem> items = new List<MenuFlyoutItem>();
                        foreach (var i in actions.ContextMenuItems)
                        {
                            var menuItem = new MenuFlyoutItem()
                            {
                                Text = i.Content
                            };

                            menuItem.Click += delegate
                            {
                                TriggerActivation(i.Arguments, i.ActivationType);
                            };

                            items.Add(menuItem);
                        }

                        AssignMenuFlyout(items.ToArray());
                    }
                }
            }

            OnIsExpandedChanged(null);
        }

        private T AssignId<T>(T uiElement, string id) where T : FrameworkElement
        {
            if (id != null)
            {
                uiElement.Name = id;
                _elementsWithIds[id] = uiElement;
            }

            return uiElement;
        }

        private Thickness GetInputAndButtonMargin()
        {
            if (CurrFeatureSet.RS3_Style_Toasts)
            {
                return new Thickness(0, 0, 0, GetMarginBelowInputOrButton());
            }

            return new Thickness(0, 0, 0, GetMarginBelowInputOrButton());
        }

        private int GetMarginBelowInputOrButton()
        {
            return CurrFeatureSet.RS3_Style_Toasts ? 16 : 12;
        }

        private TextBox CreateTextBox(string id, string title, string defaultInput, string placeHolderContent, bool hintVisible)
        {
            TextBox tb = new TextBox()
            {
                Margin = GetInputAndButtonMargin(),
                Tag = id,
                AcceptsReturn = true,
                PlaceholderForeground = new SolidColorBrush(Colors.Gray)
            };

            if (defaultInput != null)
                tb.Text = defaultInput;

            if (placeHolderContent != null)
                tb.PlaceholderText = placeHolderContent;

            if (title != null)
                tb.Header = title;

            _currInputs.Add(new TextInput(tb));

            if (id != null)
                _elementsWithIds[id] = tb;

            if (!hintVisible)
                tb.Visibility = Visibility.Collapsed;

            return tb;
        }

        private ComboBox CreateComboBox(string id, string title, Selection[] items, string defaultInput, bool hintVisible)
        {
            ComboBox cb = new ComboBox()
            {
                Margin = GetInputAndButtonMargin(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = id
            };

            if (title != null)
                cb.Header = title;

            cb.ItemsSource = items;

            if (defaultInput != null)
                cb.SelectedItem = items.FirstOrDefault(x => x.Id.Equals(defaultInput));

            _currInputs.Add(new SelectionInput(cb));

            if (id != null)
                _elementsWithIds[id] = cb;

            if (!hintVisible)
                cb.Visibility = Visibility.Collapsed;

            return cb;
        }

        private ValueSet GetCurrentUserInput()
        {
            ValueSet answer = new ValueSet();

            foreach (var input in _currInputs)
            {
                answer.Add(input.Id, input.Value);
            }

            return answer;
        }

        private Button CreateButton(string content, string arguments, string imageUri, ActivationType activationType, Scenario scenarioType)
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

            Button b;

            switch (scenarioType)
            {
                case Scenario.IncomingCall:
                    b = new CallingButton()
                    {
                        Text = content,
                        ImageUri = imageUri
                    };
                    break;

                default:
                    b = new Button()
                    {
                        Content = content,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Margin = GetInputAndButtonMargin(),
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    break;

            }

            b.Click += delegate
            {
                TriggerActivation(arguments, activationType);
            };

            return b;
        }

        private FrameworkElement CreateImageButton(string src, string arguments, ActivationType activationType)
        {
            Image uiButton = new Image()
            {
                Source = ImageHelper.GetBitmap(src),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(12, 0, 0, GetMarginBelowInputOrButton()),
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 32,
                Height = 32
            };

            uiButton.Tapped += (s, e) =>
            {
                e.Handled = true;
                TriggerActivation(arguments, activationType);
            };

            return uiButton;
        }

        private void TriggerActivation(string arguments, ActivationType activationType, ValueSet userInputs = null)
        {
            if (arguments == null)
                arguments = "";

            if (userInputs == null)
            {
                userInputs = GetCurrentUserInput();
            }

            PreviewToastNotificationActivatedEventArgs args = new PreviewToastNotificationActivatedEventArgs(arguments, userInputs);

            switch (activationType)
            {
                case ActivationType.Background:
                    ActivatedBackground?.Invoke(this, args);
                    break;

                case ActivationType.Foreground:
                    ActivatedForeground?.Invoke(this, args);
                    break;

                case ActivationType.Protocol:
                    ActivatedProtocol?.Invoke(this, args);
                    break;

                case ActivationType.System:
                    ActivatedSystem?.Invoke(this, args);
                    break;
            }
        }

        private void ToastMainContainerButton_Click(object sender, RoutedEventArgs e)
        {
            // In RS3, Action Center started returning the inputs when body of toast clicked. So for non-RS3 we initialize an empty value set, and for
            // RS3+ we pass null which will automatically collect the inputs
            TriggerActivation(_currLaunch, _currActivationType, userInputs: CurrFeatureSet.RS3_Style_Toasts ? null : new ValueSet());
        }

        private void CloseIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void AddInput(UIElement el)
        {
            StackPanelInputs.Children.Add(el);
        }

        private void AddButton(FrameworkElement el)
        {
            ButtonsContainer.Items.Add(el);
        }

#region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewToast), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

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
            (sender as PreviewToast).OnDeviceFamilyChanged(e);
        }

        private void OnDeviceFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Feature set is affected
            UpdateFeatureSet();
        }

#endregion

        private static readonly DependencyProperty OSBuildNumberProperty = DependencyProperty.Register("OSBuildNumber", typeof(Version), typeof(PreviewToast), new PropertyMetadata(FeatureSet.GetCurrentOSBuildNumber(), OnOSBuildNumberChanged));

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
            (sender as PreviewToast).OnOSBuildNumberChanged(e);
        }

        private void ButtonExpandCollapse_Click(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        private void OnOSBuildNumberChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateFeatureSet();
        }

        internal FeatureSet CurrFeatureSet { get; private set; }

        private void UpdateFeatureSet()
        {
            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            if (CurrFeatureSet.R_19H2_Style_Toasts)
            {
                VisualStateManager.GoToState(this, "Current", false);
            }
            else if (CurrFeatureSet.RS3_Style_Toasts)
            {
                VisualStateManager.GoToState(this, "Pre19H2", false);
            }
            else if (CurrFeatureSet.RS1_Style_Toasts)
            {
                VisualStateManager.GoToState(this, "PreRS3", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "PreRS1", false);
            }

            this.ReInitializeContent();
        }
    }
}
