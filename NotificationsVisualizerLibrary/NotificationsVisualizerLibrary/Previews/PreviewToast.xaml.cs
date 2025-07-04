using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Controls;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.Foundation.Metadata;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public sealed class PreviewToastProperties
    {
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 120, 215);

        public Uri Square44x44Logo { get; set; }

        public String DisplayName { get; set; }
    }

    public sealed class PreviewToastNotificationActivatedEventArgs
    {
        internal PreviewToastNotificationActivatedEventArgs(String argument, ValueSet userInput)
        {
            this.Argument = argument;
            this.UserInput = userInput;
        }

        public String Argument { get; private set; }

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

            this.CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            this.TextBlockAttributionFirstPart.SetBinding(TextBlock.TextProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath("Properties.DisplayName")
            });

            this.AppName.SetBinding(TextBlock.TextProperty, new Binding()
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
            this.AssignMenuFlyout(null);
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
                        Text = "Turn off notifications for " + this.Properties?.DisplayName
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
            return this.Initialize(content, new PreviewNotificationData());
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

        /// <summary>
        /// Call this to update the toast's data.
        /// </summary>
        /// <param name="data"></param>
        public void Update(PreviewNotificationData data)
        {
            if (this._lastDataBindingValues == null || !(this._currContent is Toast) || data.Version < this._lastDataBindingValues.Version)
            {
                return;
            }
            this._lastDataBindingValues.Update(data.Values);
            (this._currContent as Toast).ApplyDataBinding(this._lastDataBindingValues);
        }

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewToastProperties), typeof(PreviewToast), new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get { return this.GetValue(PropertiesProperty) as PreviewToastProperties; }
            set { this.SetValue(PropertiesProperty, value); }
        }

        private static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(Boolean), typeof(PreviewToast), new PropertyMetadata(true, OnIsExpandedChanged));

        private static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewToast).OnIsExpandedChanged(e);
        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsExpanded)
            {
                if (this.CurrFeatureSet != null && this.CurrFeatureSet.RS1_Style_Toasts)
                    VisualStateManager.GoToState(this, "ExpandedAdaptiveState", false);
                else
                    VisualStateManager.GoToState(this, "ExpandedState", false);
            }

            else
            {
                if (this.CurrFeatureSet != null && this.CurrFeatureSet.RS1_Style_Toasts)
                    VisualStateManager.GoToState(this, "CollapsedStateRS1", false);
                else
                    VisualStateManager.GoToState(this, "CollapsedState", false);
            }
        }

        public Boolean IsExpanded
        {
            get { return (Boolean)this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        private interface IInput
        {
            String Id { get; }
            String Value { get; }
        }

        private class TextInput : IInput
        {
            private TextBox _tb;
            public TextInput(TextBox tb)
            {
                this._tb = tb;
            }

            public String Id
            {
                get
                {
                    return (String)this._tb.Tag;
                }
            }

            public String Value
            {
                get
                {
                    return this._tb.Text;
                }
            }
        }

        private class SelectionInput : IInput
        {
            private ComboBox _cb;

            public SelectionInput(ComboBox cb)
            {
                this._cb = cb;
            }

            public String Id
            {
                get
                {
                    return (String)this._cb.Tag;
                }
            }

            public String Value
            {
                get
                {
                    var selection = this._cb.SelectedItem as Selection;

                    if (selection != null)
                        return selection.Id;

                    return "";
                }
            }
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

        private void ResetContent()
        {
            this.HasContent = false;

            this.TextBlockTitle.Text = "";
            this.TextBlockTitle.MaxLines = 2;
            this.TextBlockTitle.TextWrapping = TextWrapping.WrapWholeWords;

            // In RS3+, title text is SemiBold (Base style)
            // In RS1-RS2, title text isn't bold (Body style)
            // In Pre-RS1, title text was SemiBold
            if (this.CurrFeatureSet.RS3_Style_Toasts)
            {
                this.TextBlockTitle.Style = (Style)this.Resources["BaseTextBlockStyle"];
            }
            else if (this.CurrFeatureSet.RS1_Style_Toasts)
            {
                this.TextBlockTitle.Style = (Style)this.Resources["BodyTextBlockStyle"];
            }
            else
            {
                this.TextBlockTitle.Style = (Style)this.Resources["BaseTextBlockStyle"];
            }

            this.TextBlockBody.Text = "";
            this.TextBlockBody.MaxLines = 4;
            this.TextBlockBody.TextWrapping = TextWrapping.WrapWholeWords;

            this.TextBlockHeaderWhenAppLogoNotPresent.Text = "";
            this.TextBlockHeaderWhenAppLogoPresent.Text = "";

            this.ContentAdaptive.Child = null;

            this.ButtonsContainer.Items.Clear();

            this.StackPanelInlineImages.Children.Clear();
            this.StackPanelInputs.Children.Clear();

            this.CallingImageContainer.Child = null;

            this.Pre19H2AppLogo.Visibility = this.CurrFeatureSet.R_19H2_Style_Toasts ? Visibility.Collapsed : Visibility.Visible;
            this.ImageAppLogo.Visibility = Visibility.Collapsed;
            this.CircleImageAppLogo.Visibility = Visibility.Collapsed;

            this._elementsWithIds = new Dictionary<String, FrameworkElement>();

            this._currInputs.Clear();
            this._currLaunch = "";
            this._currActivationType = ActivationType.Foreground;

            this.AssignMenuFlyout(null);

            this.StackPanelAttribution.Visibility =Visibility.Collapsed;
            this.TextBlockAttributionFirstPart.Visibility = Visibility.Collapsed;
            this.TextBlockAttributionSeparationDot.Visibility = Visibility.Collapsed;
            this.TextBlockAttributionSecondPart.Visibility = Visibility.Collapsed;
            this.AppLogoOverrideContainer.Visibility = Visibility.Collapsed;
        }

        private void InitializeContent(IToast toastContent)
        {
            this.ResetContent();

            this._currContent = toastContent;

            if (toastContent != null)
            {
                this._currLaunch = toastContent.Launch;
                this._currActivationType = toastContent.ActivationType.GetValueOrDefault(ActivationType.Foreground);

                switch (toastContent.Scenario)
                {
                    case Scenario.IncomingCall:
                        this.ButtonsContainer.ItemsPanel = (ItemsPanelTemplate)this.Resources["CallingButtonsPanelTemplate"];
                        this.ButtonsContainer.Margin = new Thickness();
                        break;

                    default:
                        this.ButtonsContainer.ItemsPanel = (ItemsPanelTemplate)this.Resources["NormalButtonsPanelTemplate"];
                        this.ButtonsContainer.Margin = this.CurrFeatureSet.RS3_Style_Toasts ? new Thickness(16, 0, 16, 0) : new Thickness(12, 0, 12, 0);
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
                        this.HasContent = true;

                        var container = binding.Container;
                        var children = new List<AdaptiveChildElement>(container.Children);


                        var texts = children.OfType<AdaptiveTextField>().ToList();
                        List<AdaptiveImage> appLogoOverrides = null;
                        var heroImages = new List<AdaptiveImage>();
                        var attributionTexts = children.OfType<AdaptiveTextField>().Where(i => i.Placement == Model.Enums.TextPlacement.Attribution).ToList();


                        if (this.CurrFeatureSet.AdaptiveToasts)
                        {
                            appLogoOverrides = new List<AdaptiveImage>();

                            // First pull out images with placements and attribution text
                            for (Int32 i = 0; i < children.Count; i++)
                            {
                                var child = children[i];

                                if (child is AdaptiveImage)
                                {
                                    var img = child as AdaptiveImage;

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
                                    var txt = child as AdaptiveTextField;

                                    if (txt.Placement != Model.Enums.TextPlacement.Inline)
                                    {
                                        children.RemoveAt(i);
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


                            texts = new List<AdaptiveTextField>();

                            // Pull out all texts
                            for (Int32 i = 0; i < children.Count; i++)
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
                            this.TextBlockTitle.Text = titleText.Text;

                            if (this.CurrFeatureSet.AdaptiveToasts)
                            {
                                this.TextBlockTitle.MaxLines = Math.Min(titleText.HintMaxLines.GetValueOrDefault(2), 2);
                                this.TextBlockTitle.TextWrapping = titleText.HintWrap.GetValueOrDefault(true) ? TextWrapping.WrapWholeWords : TextWrapping.NoWrap;
                            }

                            var bodyTextLine1 = texts.ElementAtOrDefault(1);
                            if (bodyTextLine1 != null)
                            {
                                if (this.CurrFeatureSet.AdaptiveToasts)
                                {
                                    this.TextBlockBody.Text = bodyTextLine1.Text;
                                    this.TextBlockBody.MaxLines = Math.Min(bodyTextLine1.HintMaxLines.GetValueOrDefault(4), 4);

                                    var bodyTextLine2 = texts.ElementAtOrDefault(2);
                                    if (bodyTextLine2 != null)
                                    {
                                        this.TextBlockBody.Text += "\n" + bodyTextLine2.Text;
                                    }
                                }

                                else
                                {
                                    var bodyTextLine2 = texts.ElementAtOrDefault(2);
                                    if (bodyTextLine2 != null)
                                        this.TextBlockBody.Text = bodyTextLine1.Text + "\n" + bodyTextLine2.Text;
                                    else
                                        this.TextBlockBody.Text = bodyTextLine1.Text;
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
                                        this.CallingImageContainer.Child = this.AssignId(new CircleImage()
                                        {
                                            Source = ImageHelper.GetBitmap(callingImage.Src),
                                            Width = 96,
                                            Height = 96,
                                            Margin = new Thickness(0, 0, 0, 12)
                                        }, callingImage.Id);
                                        break;

                                    default:
                                        this.CallingImageContainer.Child = this.AssignId(new Image()
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
                            appLogoOverrides = [.. images.Where(i => i.Placement == Model.Enums.Placement.AppLogoOverride)];

                        var appLogoOverride = appLogoOverrides.FirstOrDefault();
                        if (appLogoOverride != null)
                        {
                            this.Pre19H2AppLogo.Visibility = Visibility.Collapsed;
                            this.AppLogoOverrideContainer.Visibility = Visibility.Visible;

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

                            if (!this.CurrFeatureSet.R_19H2_Style_Toasts && this.CurrFeatureSet.RS1_Style_Toasts)
                            {
                                // We also show the attribution app name
                                this.StackPanelAttribution.Visibility = Visibility.Visible;
                                this.TextBlockAttributionFirstPart.Visibility = Visibility.Visible;
                            }
                        }

                        if (toastContent.Header != null)
                        {
                            if (appLogoOverride != null)
                            {
                                this.TextBlockHeaderWhenAppLogoPresent.Text = toastContent.Header.Title;
                            }
                            else
                            {
                                this.TextBlockHeaderWhenAppLogoNotPresent.Text = toastContent.Header.Title;
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

                            this.StackPanelInlineImages.Children.Add(uiImage);
                        }


                        // Initialize adaptive content
                        if (toastContent.SupportedFeatures.AdaptiveToasts && toastContent.Scenario != Scenario.IncomingCall)
                        {
                            if (children.Count > 0)
                            {
                                // Move any progress bars to the top
                                if (children.OfType<AdaptiveProgress>().Any())
                                {
                                    Int32 insertProgressIndex = 0;
                                    for (Int32 i = 0; i < children.Count; i++)
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

                                var notif = new PreviewTileNotification()
                                {
                                };
                                notif.InitializeFromXml(TileSize.Large, new PreviewTileVisualElements()
                                {
                                    BackgroundColor = Colors.Transparent
                                }, isBrandingVisible: false,
                                binding: binding);

                                binding.Container.SwapChildren(oldChildren);

                                this.ContentAdaptive.Child = notif;
                            }
                        }



                        // Attribution
                        if (toastContent.SupportedFeatures.RS1_Style_Toasts)
                        {
                            if (attributionTexts.Count != 0)
                            {
                                this.StackPanelAttribution.Visibility = Visibility.Visible;

                                if (this.TextBlockAttributionFirstPart.Visibility == Visibility.Visible)
                                {
                                    this.TextBlockAttributionSeparationDot.Visibility = Visibility.Visible;
                                }

                                this.TextBlockAttributionSecondPart.Visibility = Visibility.Visible;
                                this.TextBlockAttributionSecondPart.Text = attributionTexts.First().Text;
                            }
                        }
                    }
                }


                if (toastContent.Actions != null)
                {
                    var actions = toastContent.Actions;

                    if (actions.HintSystemCommands == Model.Enums.HintSystemCommands.SnoozeAndDismiss)
                    {
                        this.AddInput(this.CreateComboBox("systemSnoozeSelection", null, new Selection[]
                        {
                            new Selection(NotificationType.Toast, this.CurrFeatureSet)
                            {
                                Content = "5 minutes",
                                Id = "5"
                            },

                            new Selection(NotificationType.Toast, this.CurrFeatureSet)
                            {
                                Content = "9 minutes",
                                Id = "9"
                            },

                            new Selection(NotificationType.Toast, this.CurrFeatureSet)
                            {
                                Content = "10 minutes",
                                Id = "10"
                            },

                            new Selection(NotificationType.Toast, this.CurrFeatureSet)
                            {
                                Content = "1 hour",
                                Id = "60"
                            },

                            new Selection(NotificationType.Toast, this.CurrFeatureSet)
                            {
                                Content = "4 hours",
                                Id = "240"
                            },

                            new Selection(NotificationType.Toast, this.CurrFeatureSet)
                            {
                                Content = "1 day",
                                Id = "1440"
                            }
                        }, "9", true));

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
                            // See if we can pull out a related action to display on side
                            Model.BaseElements.Action a = null;

                            // We only place ajacent to text boxes
                            if (i.Type == InputType.Text)
                                a = actionElements.FirstOrDefault(x => Object.Equals(x.InputId, i.Id));

                            if (a != null)
                                actionElements.Remove(a);

                            var grid = new Grid();

                            if (a != null)
                            {
                                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                                var uiButton = this.CreateImageButton(a.ImageUri, a.Arguments, a.ActivationType);

                                this.AssignId(uiButton, a.Id);

                                if (!a.HintVisible)
                                    uiButton.Visibility = Visibility.Collapsed;

                                Grid.SetColumn(uiButton, 1);
                                grid.Children.Add(uiButton);
                            }

                            switch (i.Type)
                            {
                                case Model.BaseElements.InputType.Text:

                                    TextBox tb = this.CreateTextBox(
                                        id: i.Id,
                                        title: i.Title,
                                        defaultInput: i.DefaultInput,
                                        placeHolderContent: i.PlaceHolderContent,
                                        hintVisible: i.HintVisible);

                                    grid.Children.Add(tb);
                                    break;

                                case Model.BaseElements.InputType.Selection:

                                    ComboBox cb = this.CreateComboBox(i.Id, i.Title, i.Children.OfType<Selection>().ToArray(), i.DefaultInput, i.HintVisible);

                                    grid.Children.Add(cb);

                                    break;

                                default:
                                    throw new NotImplementedException();
                            }

                            this.AddInput(grid);
                        }

                        // If there's only one button
                        if (actionElements.Count == 1 && toastContent.Scenario != Scenario.IncomingCall)
                        {
                            Model.BaseElements.Action a = actionElements[0];

                            Button uiButton = this.CreateButton(a.Content, a.Arguments, a.ImageUri, a.ActivationType, toastContent.Scenario);

                            this.AssignId(uiButton, a.Id);

                            if (!a.HintVisible)
                                uiButton.Visibility = Visibility.Collapsed;

                            if (this.CurrFeatureSet.R_19H1_Style_Toasts)
                            {
                                // In 19H1 (and possibly even earlier), single buttons stretch full width...
                                // so do nothing at all!
                            }

                            else if (this.CurrFeatureSet.RS3_Style_Toasts
                                || !this.CurrFeatureSet.RS1_Style_Toasts)
                            {
                                // In RS3 and higher, and also before RS1, we have it consume half the width on the right
                                // by adding a blank entry first
                                this.AddButton(new Border());
                            }

                            else
                            {
                                // In Anniversary Update the button will increase to fit the text
                                uiButton.HorizontalAlignment = HorizontalAlignment.Right;
                            }

                            // Then add the actual button (so it appears on the right half)
                            this.AddButton(uiButton);
                        }

                        else
                        {
                            // Initialize buttons
                            foreach (var a in actionElements)
                            {
                                Button uiButton = this.CreateButton(a.Content, a.Arguments, a.ImageUri, a.ActivationType, toastContent.Scenario);

                                this.AssignId(uiButton, a.Id);

                                if (!a.HintVisible)
                                    uiButton.Visibility = Visibility.Collapsed;

                                this.AddButton(uiButton);
                            }
                        }
                    }

                    if (actions.ContextMenuItems != null && actions.ContextMenuItems.Any())
                    {
                        var items = new List<MenuFlyoutItem>();
                        foreach (var i in actions.ContextMenuItems)
                        {
                            var menuItem = new MenuFlyoutItem()
                            {
                                Text = i.Content
                            };

                            menuItem.Click += delegate
                            {
                                this.TriggerActivation(i.Arguments, i.ActivationType);
                            };

                            items.Add(menuItem);
                        }

                        this.AssignMenuFlyout([.. items]);
                    }
                }
            }

            this.OnIsExpandedChanged(null);
        }

        private T AssignId<T>(T uiElement, String id) where T : FrameworkElement
        {
            if (id != null)
            {
                uiElement.Name = id;
                this._elementsWithIds[id] = uiElement;
            }

            return uiElement;
        }

        private Thickness GetInputAndButtonMargin()
        {
            if (this.CurrFeatureSet.RS3_Style_Toasts)
            {
                return new Thickness(0, 0, 0, this.GetMarginBelowInputOrButton());
            }

            return new Thickness(0, 0, 0, this.GetMarginBelowInputOrButton());
        }

        private Int32 GetMarginBelowInputOrButton()
        {
            return this.CurrFeatureSet.RS3_Style_Toasts ? 16 : 12;
        }

        private TextBox CreateTextBox(String id, String title, String defaultInput, String placeHolderContent, Boolean hintVisible)
        {
            var tb = new TextBox()
            {
                Margin = this.GetInputAndButtonMargin(),
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

            this._currInputs.Add(new TextInput(tb));

            if (id != null)
                this._elementsWithIds[id] = tb;

            if (!hintVisible)
                tb.Visibility = Visibility.Collapsed;

            return tb;
        }

        private ComboBox CreateComboBox(String id, String title, Selection[] items, String defaultInput, Boolean hintVisible)
        {
            var cb = new ComboBox()
            {
                Margin = this.GetInputAndButtonMargin(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = id
            };

            if (title != null)
                cb.Header = title;

            cb.ItemsSource = items;

            if (defaultInput != null)
                cb.SelectedItem = items.FirstOrDefault(x => x.Id.Equals(defaultInput));

            this._currInputs.Add(new SelectionInput(cb));

            if (id != null)
                this._elementsWithIds[id] = cb;

            if (!hintVisible)
                cb.Visibility = Visibility.Collapsed;

            return cb;
        }

        private ValueSet GetCurrentUserInput()
        {
            var answer = new ValueSet();

            foreach (var input in this._currInputs)
            {
                answer.Add(input.Id, input.Value);
            }

            return answer;
        }

        private Button CreateButton(String content, String arguments, String imageUri, ActivationType activationType, Scenario scenarioType)
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
                        Margin = this.GetInputAndButtonMargin(),
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    break;

            }

            b.Click += delegate
            {
                this.TriggerActivation(arguments, activationType);
            };

            return b;
        }

        private FrameworkElement CreateImageButton(String src, String arguments, ActivationType activationType)
        {
            var uiButton = new Image()
            {
                Source = ImageHelper.GetBitmap(src),
                Stretch = Stretch.Uniform,
                Margin = new Thickness(12, 0, 0, this.GetMarginBelowInputOrButton()),
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 32,
                Height = 32
            };

            uiButton.Tapped += (s, e) =>
            {
                e.Handled = true;
                this.TriggerActivation(arguments, activationType);
            };

            return uiButton;
        }

        private void TriggerActivation(String arguments, ActivationType activationType, ValueSet userInputs = null)
        {
            if (arguments == null)
                arguments = "";

            if (userInputs == null)
            {
                userInputs = this.GetCurrentUserInput();
            }

            var args = new PreviewToastNotificationActivatedEventArgs(arguments, userInputs);

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

        private void ToastMainContainerButton_Click(Object sender, RoutedEventArgs e)
        {
            // In RS3, Action Center started returning the inputs when body of toast clicked. So for non-RS3 we initialize an empty value set, and for
            // RS3+ we pass null which will automatically collect the inputs
            this.TriggerActivation(this._currLaunch, this._currActivationType, userInputs: this.CurrFeatureSet.RS3_Style_Toasts ? null : new ValueSet());
        }

        private void CloseIcon_Tapped(Object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void AddInput(UIElement el)
        {
            this.StackPanelInputs.Children.Add(el);
        }

        private void AddButton(FrameworkElement el)
        {
            this.ButtonsContainer.Items.Add(el);
        }

#region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewToast), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

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
            (sender as PreviewToast).OnDeviceFamilyChanged(e);
        }

        private void OnDeviceFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Feature set is affected
            this.UpdateFeatureSet();
        }

#endregion

        private static readonly DependencyProperty OSBuildNumberProperty = DependencyProperty.Register("OSBuildNumber", typeof(Version), typeof(PreviewToast), new PropertyMetadata(FeatureSet.GetCurrentOSBuildNumber(), OnOSBuildNumberChanged));

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
            (sender as PreviewToast).OnOSBuildNumberChanged(e);
        }

        private void ButtonExpandCollapse_Click(Object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void OnOSBuildNumberChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateFeatureSet();
        }

        internal FeatureSet CurrFeatureSet { get; private set; }

        private void UpdateFeatureSet()
        {
            this.CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            if (this.CurrFeatureSet.R_19H2_Style_Toasts)
            {
                VisualStateManager.GoToState(this, "Current", false);
            }
            else if (this.CurrFeatureSet.RS3_Style_Toasts)
            {
                VisualStateManager.GoToState(this, "Pre19H2", false);
            }
            else if (this.CurrFeatureSet.RS1_Style_Toasts)
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
