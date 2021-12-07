using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Manifest;
using NotificationsVisualizerLibrary.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using NotificationsVisualizerLibrary.Model.Enums;
using Windows.UI.Xaml.Media.Imaging;
using NotificationsVisualizerLibrary.Controls;
using System.Diagnostics;
using Windows.Storage;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Helpers;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Shapes;
using NotificationsVisualizerLibrary.Renderers;
using NotificationsVisualizerLibrary.Parsers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public enum DeviceFamily
    {
        Desktop,
        Mobile
    }

    public sealed partial class PreviewTile : UserControl
    {
        private PreviewTileUpdater _tileUpdater;
        private PreviewBadgeUpdater _badgeUpdater;

        public PreviewTile()
        {
            this.InitializeComponent();

            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            Initialize();
        }

        private async void Initialize()
        {
            _tileUpdater = new PreviewTileUpdater(this);
            _badgeUpdater = new PreviewBadgeUpdater(this);

            UpdateTileSize();

            // TODO - automatically look for their Package.appxmanifest, and initialize VisualElements data from there.

            await UpdateAsync();

            // Initialize the default tile
            Show(null, false);
        }

        #region Platform

        private static readonly DependencyProperty PlatformProperty;

        #endregion

        #region TileSize

        private static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register(
            "TileSize", typeof(TileSize), typeof(PreviewTile), new PropertyMetadata(TileSize.Medium, OnTileSizeChanged));

        private static void OnTileSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PreviewTile).OnTileSizeChanged(e);
        }

        private void OnTileSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateTileSize();

            //re-render, since different binding might be used
            Reshow();
        }

        private void UpdateTileSize()
        {
            switch (TileSize)
            {
                case TileSize.Small:
                    this.TilePixelSize = TileDensity.Small;
                    break;

                case TileSize.Medium:
                    this.TilePixelSize = TileDensity.Medium;
                    break;

                case TileSize.Wide:
                    this.TilePixelSize = TileDensity.Wide;
                    break;

                case TileSize.Large:
                    this.TilePixelSize = TileDensity.Large;
                    break;

                default:
                    throw new NotImplementedException(TileSize.ToString());
            }

            UpdateBranding();
        }

        public TileSize TileSize
        {
            get { return (TileSize)GetValue(TileSizeProperty); }
            set { SetValue(TileSizeProperty, value); }
        }

        #endregion

        #region TileDensity

        private static readonly DependencyProperty TileDensityProperty = DependencyProperty.Register("TileDensity", typeof(TileDensity), typeof(PreviewTile), new PropertyMetadata(TileDensity.Desktop(), OnTileDensityChanged));

        private static void OnTileDensityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewTile).OnTileDensityChanged(e);
        }

        private void OnTileDensityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TileDensity == null)
                throw new NullReferenceException("TileDensity cannot be null");

            // It automatically scales
            UpdateTileSize();
        }

        public TileDensity TileDensity
        {
            get { return GetValue(TileDensityProperty) as TileDensity; }
            set { SetValue(TileDensityProperty, value); }
        }

        #endregion

        #region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty = DependencyProperty.Register("DeviceFamily", typeof(DeviceFamily), typeof(PreviewTile), new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

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
            (sender as PreviewTile).OnDeviceFamilyChanged(e);
        }

        private void OnDeviceFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Branding is potentially affected
            UpdateBranding();

            // And so is feature set
            UpdateFeatureSet();
        }

        #endregion

        private static readonly DependencyProperty OSBuildNumberProperty = DependencyProperty.Register("OSBuildNumber", typeof(int), typeof(PreviewTile), new PropertyMetadata(FeatureSet.GetCurrentOSBuildNumber(), OnOSBuildNumberChanged));

        /// <summary>
        /// Gets or sets the current OS version, which impacts what features are available in the adaptive tiles (and bug fixes).
        /// </summary>
        public int OSBuildNumber
        {
            get { return (int)GetValue(OSBuildNumberProperty); }
            set { SetValue(OSBuildNumberProperty, value); }
        }

        private static void OnOSBuildNumberChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewTile).OnOSBuildNumberChanged(e);
        }

        private void OnOSBuildNumberChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateFeatureSet();
        }

        internal FeatureSet CurrFeatureSet { get; private set; }

        private void UpdateFeatureSet()
        {
            CurrFeatureSet = FeatureSet.Get(this.DeviceFamily, this.OSBuildNumber);

            this.UpdateBranding();
            this.UpdateDisplayName();
            this.UpdateVisualElements();
            this.Reshow();
        }

        private static readonly DependencyProperty TilePixelSizeProperty = DependencyProperty.Register("TilePixelSize", typeof(Size), typeof(PreviewTile), new PropertyMetadata(default(Size)));

        private Size TilePixelSize
        {
            get { return (Size)GetValue(TilePixelSizeProperty); }
            set { SetValue(TilePixelSizeProperty, value); }
        }

        private static readonly DependencyProperty IsAnimationEnabledProperty = DependencyProperty.Register("IsAnimationEnabled", typeof(bool), typeof(PreviewTile), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether animation is enabled. If true, when new tile updates appear, they will animate onto the tile. Otherwise, they will just instantly appear.
        /// </summary>
        public bool IsAnimationEnabled
        {
            get { return (bool)GetValue(IsAnimationEnabledProperty); }
            set { SetValue(IsAnimationEnabledProperty, value); }
        }

        private Model.Enums.Template[] GetValidTemplateValues()
        {
            switch (TileSize)
            {
                case TileSize.Small:
                    return new Model.Enums.Template[] { Model.Enums.Template.TileSmall };

                case TileSize.Medium:
                    return new Model.Enums.Template[] { Model.Enums.Template.TileMedium };

                case TileSize.Wide:
                    return new Model.Enums.Template[] { Model.Enums.Template.TileWide };

                case TileSize.Large:
                    return new Model.Enums.Template[] { Model.Enums.Template.TileLarge };

                default:
                    return new Model.Enums.Template[0];
            }
        }

        private Package _package;
        private object _notificationData;

        private Uri GetLogo()
        {
            switch (TileSize)
            {
                case TileSize.Small:
                    return VisualElements.Square71x71Logo ?? VisualElements.Square150x150Logo;

                case TileSize.Medium:
                    return VisualElements.Square150x150Logo;

                case TileSize.Wide:
                    return VisualElements.Wide310x150Logo;

                case TileSize.Large:
                    return VisualElements.Square310x310Logo ?? VisualElements.Square150x150Logo;

                default:
                    throw new NotImplementedException();
            }
        }

        private Uri GetCornerLogo()
        {
            return VisualElements.Square44x44Logo;
        }

        private static async Task<string> GetMrtUri(Package package, string path)
        {
            return await package.GetMrtUri(path);
        }

        /// <summary>
        /// Returns a tile updater for updating the content of this preview tile.
        /// </summary>
        /// <returns></returns>
        public PreviewTileUpdater CreateTileUpdater()
        {
            return _tileUpdater;
        }

        /// <summary>
        /// Returns a badge updater for updating the badge for this preview tile.
        /// </summary>
        /// <returns></returns>
        public PreviewBadgeUpdater CreateBadgeUpdater()
        {
            return _badgeUpdater;
        }

        private void ResetDisplayProperties()
        {
            // Update display name back to its original tile-based one (clearing any display name set by a notification)
            _customDisplayName = null;
            UpdateDisplayName();

            // Update branding to default
            _notificationBranding = null;
            _hasNotificationForCurrentSize = false;
            UpdateBranding();
        }

        private void ShowDefault(bool animate)
        {
            // Show logo
            ShowElement(new Border()
            {
                Background = new SolidColorBrush(VisualElements.BackgroundColor),
                Child = new Windows.UI.Xaml.Controls.Image()
                {
                    Source = GenerateBitmapImage(GetLogo()),
                    Stretch = Stretch.UniformToFill
                }
            }, animate);
        }

        private BitmapImage GenerateBitmapImage(Uri uri)
        {
            if (uri == null)
                return null;

            return ImageHelper.GetBitmap(uri.OriginalString);
        }
        

        private Branding? _notificationBranding;
        private bool _hasNotificationForCurrentSize;

        private void UpdateBranding()
        {
            // On Small tiles, margin is only 4px, so branding needs to move too
            switch (TileSize)
            {
                case TileSize.Small:
                    Branding.Margin = new Thickness(0, 0, 2, -4);
                    break;

                default:
                    Branding.Margin = new Thickness(0, 0, 6, 0);
                    break;
            }

            bool showName;
            bool showCornerLogo;

            // If the notification overrided branding
            if (_notificationBranding != null)
            {
                var brandingValue = _notificationBranding.Value;

                // If we're on Mobile
                if (DeviceFamily == DeviceFamily.Mobile)
                {
                    // "logo" and "nameAndLogo" become "name"
                    switch (brandingValue)
                    {
                        case Model.Enums.Branding.Logo:
                        case Model.Enums.Branding.NameAndLogo:
                            brandingValue = Model.Enums.Branding.Name;
                            break;
                    }
                }

                switch (brandingValue)
                {
                    case Model.Enums.Branding.None:
                        showName = false;
                        showCornerLogo = false;
                        break;

                    case Model.Enums.Branding.Name:
                        showName = true;
                        showCornerLogo = false;
                        break;

                    case Model.Enums.Branding.Logo:
                        showName = false;
                        showCornerLogo = true;
                        break;

                    default: //NameAndLogo
                        showName = true;
                        showCornerLogo = true;
                        break;
                }

                // Small can never show display name (but it can apparently display logo)
                if (TileSize == TileSize.Small)
                    showName = false;
            }

            // Otherwise, use behaviors from VisualElements
            else
            {
                // Default behavior is to never show corner logo (if notification displayed, default branding inherits ShowName from basic tile properties, never shows corner logo)
                showCornerLogo = false;

                switch (TileSize)
                {
                    case TileSize.Small:
                        showName = false; // name never shown on small
                        break;

                    case TileSize.Medium:
                        showName = VisualElements.ShowNameOnSquare150x150Logo;
                        break;

                    case TileSize.Wide:
                        showName = VisualElements.ShowNameOnWide310x150Logo;
                        break;

                    case TileSize.Large:
                        showName = VisualElements.ShowNameOnSquare310x310Logo;
                        break;

                    default:
                        throw new NotImplementedException(TileSize.ToString());
                }
            }

            // Decide to show the name
            TextBlockDisplayName.Visibility = showName ? Visibility.Visible : Visibility.Collapsed;

            // Decide to show corner logo
            CornerLogo.Visibility = showCornerLogo ? Visibility.Visible : Visibility.Collapsed;


            // Decide if branding is shown...
            if (HasBadge() || showName || showCornerLogo)
                Branding.Visibility = Visibility.Visible;
            else
                Branding.Visibility = Visibility.Collapsed;
        }

        private bool HasBadge()
        {
            return badgeValueControl.Value.HasBadge();
        }

        private void Reshow()
        {
            Show(_notificationData, false);
        }

        internal void Show(object tile, bool animate)
        {
            // Ensure valid object passed in
            if (tile != null && !(tile is ITile))
            {
                throw new InvalidOperationException("tile must be of type ITile");
            }

            // If animations are disabled, set animate to false
            if (!IsAnimationEnabled)
                animate = false;

            // Store the current tile data, so that when the size changes, we can re-render using the data
            _notificationData = tile;

            // If we're already animating, we'll wait till current animation is done, and then render/animate the new content
            if (animate && _isAnimating)
            {
                _isWaitingToShow = true;
                return;
            }

            // Otherwise we'll render/animate now
            _isWaitingToShow = false;

            // Reset things that a previous notification might have changed, like DisplayName
            ResetDisplayProperties();


            // If nothing to display, revert to default
            if (tile == null)
            {
                ShowDefault(animate);
                return;
            }

            // Find the first matching template for the current size, or if none, revert to default
            if (tile is ITile)
            {
                var xmlTile = tile as ITile;

                var binding = xmlTile.Visual.Bindings.FirstOrDefault(m => GetValidTemplateValues().Contains(m.Template));
                if (binding == null)
                {
                    ShowDefault(animate);
                    return;
                }

                _hasNotificationForCurrentSize = true;


                // Custom display name from visual level
                if (xmlTile.Visual.DisplayName != null)
                {
                    _customDisplayName = xmlTile.Visual.DisplayName;
                    UpdateDisplayName();
                }

                // Custom display name from binding level (which overrides visual)
                if (binding.DisplayName != null)
                {
                    _customDisplayName = binding.DisplayName;
                    UpdateDisplayName();
                }



                // Update branding from notification
                _notificationBranding = xmlTile.Visual.Branding; // first attempt to use branding specified in the visual

                if (binding.Branding != null)
                    _notificationBranding = binding.Branding; // and then branding has a chance to override it

                UpdateBranding();


                // Generate the actual notification content
                PreviewTileNotification notificationContent = new PreviewTileNotification()
                {
                    RequestedTheme = ElementTheme.Dark
                };
                notificationContent.InitializeFromXml(
                    tileSize: TileSize,
                    //tilePixelSize: TilePixelSize,
                    binding: binding,
                    visualElements: VisualElements,
                    isBrandingVisible: Branding.Visibility == Visibility.Visible);

                // And then show it
                ShowElement(notificationContent, animate);
            }
        }
        
        private bool _isAnimating;
        private bool _isWaitingToShow;

        /// <summary>
        /// Animates the tile to show this new content
        /// </summary>
        /// <param name="el"></param>
        private void ShowElement(UIElement el, bool animate)
        {
            UIElement previous = canvas.Children.LastOrDefault();

            // If we're not animating, or no previous content, clear children and instantly show new content
            if (!animate || previous == null)
            {
                canvas.Children.Clear();
                canvas.Children.Add(el);
                return;
            }

            // Otherwise, clear all but previous
            while (canvas.Children.Count > 1)
                canvas.Children.RemoveAt(0);

            // And then add our new
            canvas.Children.Add(el);


            //Storyboard s = CreateAnimationForAlreadySeenNotification(el);
            Storyboard s = CreateAnimationForNewNotification(previous, el);

            s.Completed += delegate
            {
                _isAnimating = false;

                // Remove our previous
                canvas.Children.Remove(previous);

                if (_isWaitingToShow)
                {
                    Show(_notificationData, animate);
                }
            };

            _isAnimating = true;
            s.Begin();
        }

        private Storyboard CreateAnimationForNewNotification(UIElement oldNotification, UIElement newNotification)
        {
            Storyboard s = new Storyboard();
            
            TimeSpan duration = TimeSpan.FromSeconds(0.5);
            TimeSpan halfTime = TimeSpan.FromSeconds(duration.TotalSeconds / 2);


            // Assign a scale transform to the old notification
            oldNotification.Projection = new PlaneProjection();

            // Make the height decrease
            DoubleAnimation a = new DoubleAnimation()
            {
                Duration = halfTime,
                To = 90,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(a, oldNotification.Projection);
            Storyboard.SetTargetProperty(a, "RotationX");
            s.Children.Add(a);





            // For new notification...
            newNotification.Projection = new PlaneProjection()
            {
                RotationX = -90
            };

            // Make the height increase
            a = new DoubleAnimation()
            {
                Duration = halfTime,
                BeginTime = halfTime,
                To = 0,
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(a, newNotification.Projection);
            Storyboard.SetTargetProperty(a, "RotationX");
            s.Children.Add(a);



            return s;
        }

        private Storyboard CreateAnimationForAlreadySeenNotification(UIElement alreadySeenNotification)
        {
            Storyboard s = new Storyboard();

            DoubleAnimation animateNewElement = new DoubleAnimation()
            {
                Duration = TimeSpan.FromSeconds(1),
                From = TilePixelSize.Height,
                To = 0
            };

            alreadySeenNotification.RenderTransform = new TranslateTransform()
            {
                Y = animateNewElement.From.Value
            };

            Storyboard.SetTarget(animateNewElement, alreadySeenNotification.RenderTransform);
            Storyboard.SetTargetProperty(animateNewElement, "Y");

            s.Children.Add(animateNewElement);

            return s;
        }
        
        private string _customDisplayName = null;

        private static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register("DisplayName", typeof(string), typeof(PreviewTile), new PropertyMetadata(""));

        /// <summary>
        /// Gets or sets a name that is associated with and displayed on the preview tile. This name is displayed on the tile and in the tile's tooltip. UpdateAsync must be called after changing this in order to commit the changes to the UI of the tile.
        /// </summary>
        public string DisplayName
        {
            get { return GetValue(DisplayNameProperty) as string; }
            set { SetValue(DisplayNameProperty, value); }
        }

        private void UpdateDisplayName()
        {
            if (_customDisplayName != null)
                TextBlockDisplayName.Text = _customDisplayName;

            else
                TextBlockDisplayName.Text = DisplayName;
        }

        private void UpdateVisualElements()
        {
            CornerLogo.Source = GenerateBitmapImage(GetCornerLogo());

            // Re-render notification since background color might have changed
            Reshow();
        }

        /// <summary>
        /// Gets an object through which you can get or set the preview tile's background color, tile images, and showing/hiding the display name.
        /// </summary>
        public PreviewTileVisualElements VisualElements { get; private set; } = new PreviewTileVisualElements();

        /// <summary>
        /// Commits the changes made to tile properties like DisplayName and VisualElements, causing the UI of the tile to reflect the changes.
        /// </summary>
        /// <returns></returns>
        public IAsyncAction UpdateAsync()
        {
            // This method is async to be more similar to SecondaryTile API, even though it doesn't do anything requiring awaits.

            return UpdateAsyncHelper().AsAsyncAction();
        }

        /// <summary>
        /// Make it async even though it's not, so it's more similar to secondary tile API
        /// </summary>
        /// <returns></returns>
        private Task UpdateAsyncHelper()
        {
            UpdateDisplayName();

            UpdateVisualElements();

            return Task.CompletedTask;
        }

        internal void SetBadge(BadgeValue value)
        {
            badgeValueControl.Value = value;

            // This might have affected branding height, so notification needs to re-display to adjust for the available size
            Reshow();
        }
    }
}
