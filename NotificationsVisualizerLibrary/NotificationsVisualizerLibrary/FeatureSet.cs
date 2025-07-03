using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Windows.System.Profile;
using NotificationsVisualizerLibrary.Helpers;
using Windows.Media.Core;

namespace NotificationsVisualizerLibrary
{
    internal class FeatureSet
    {
        private const Int32 R_19H2 = 19042;
        private const Int32 R_19H1 = 19041;
        private const Int32 RS4 = 17000;
        private const Int32 RS3 = 16299;
        private const Int32 RS2 = 15063;
        private const Int32 RS1 = 14393;
        private const Int32 TH2 = 10586;
        private const Int32 TH1 = 10240;

        private class SupportedInAttribute : Attribute
        {
            public Version MinVersion { get; private set; }

            public SupportedInAttribute(Int32 minBuildNumber)
            {
                this.MinVersion = new Version(10, 0, minBuildNumber, 0);
            }
        }

        private class MobileSupportedInAttribute : SupportedInAttribute
        {
            public MobileSupportedInAttribute(Int32 minBuildNumber) : base(minBuildNumber) { }
        }

        private class DesktopSupportedInAttribute : SupportedInAttribute
        {
            public DesktopSupportedInAttribute(Int32 minBuildNumber) : base(minBuildNumber) { }
        }

        public static readonly Version CURRENT_RELEASED_OS_VERSION = new Version(10, 0, R_19H2, 0);

        /// <summary>
        /// Dynamically gets the current OS version
        /// </summary>
        /// <returns></returns>
        public static Int32 GetCurrentOSBuildNumber()
        {
            return DeviceInfoHelper.GetCurrentOSBuildNumber();
        }

        private static DeviceFamily? _currentDeviceFamily;
        /// <summary>
        /// Dynamically gets the current device family
        /// </summary>
        /// <returns></returns>
        public static DeviceFamily GetCurrentDeviceFamily()
        {
            if (_currentDeviceFamily == null)
            {
                switch (AnalyticsInfo.VersionInfo.DeviceFamily)
                {
                    case "Windows.Mobile":
                        _currentDeviceFamily = DeviceFamily.Mobile;
                        break;

                    default:
                        _currentDeviceFamily = DeviceFamily.Desktop;
                        break;
                }
            }

            return _currentDeviceFamily.Value;
        }

        [SupportedIn(R_19H2)]
        public Boolean R_19H2_Style_Toasts { get; private set; }

        [SupportedIn(R_19H1)]
        public Boolean R_19H1_Style_Toasts { get; private set; }

        [SupportedIn(RS3)]
        public Boolean RS3_Style_Toasts { get; private set; }

        [SupportedIn(RS2)]
        public Boolean ToastDisplayTimestamp { get; private set; }

        [SupportedIn(RS2)]
        public Boolean ToastHeaders { get; private set; }

        [DesktopSupportedIn(RS2)]
        public Boolean ToastProgressBar { get; private set; }

        [SupportedIn(RS2)]
        public Boolean ToastTextDataBinding { get; private set; }

        [SupportedIn(RS1)]
        public Boolean ToastContextMenu { get; private set; }

        [SupportedIn(RS1)]
        public Boolean ToastAttribution { get; private set; }

        [SupportedIn(RS1)]
        public Boolean AdaptiveToasts { get; private set; }

        [SupportedIn(RS1)]
        public Boolean RS1_Style_Toasts { get; private set; }

        [SupportedIn(RS1)]
        public Boolean ChaseableTiles { get; private set; }

        [SupportedIn(TH2)]
        public Boolean BackgroundAndPeekImage { get; private set; }
        
        [SupportedIn(TH2)]
        public Boolean CropCircleOnPeekImage { get; private set; }
        
        [SupportedIn(TH2)]
        public Boolean CropCircleOnBackgroundImage { get; private set; }
        
        [MobileSupportedIn(TH1)]
        [SupportedIn(TH2)]
        public Boolean SpecialTemplatePeople { get; private set; }

        /// <summary>
        /// Bug 3749658
        /// </summary>
        [SupportedIn(TH2)]
        public Boolean CircleImageVerticalStretch { get; private set; }

        /// <summary>
        /// Bug 3159429 (overlay on binding applies to both background and peek, AND can be specified individually on each image element too)
        /// </summary>
        [SupportedIn(TH2)]
        public Boolean OverlayForBothBackgroundAndPeek { get; private set; }

        /// <summary>
        /// Bug 3660904 (Tile payloads that only have a background image should still respect the hint-overlay property (no longer require the tile to have text before the overlay gets applied)
        /// </summary>
        [SupportedIn(TH2)]
        public Boolean RespectDevSpecifiedBackgroundOverlayEvenWhenNoTextOnTile { get; private set; }



        public static FeatureSet Get(DeviceFamily deviceFamily, Int32 buildNumber)
        {
            if (buildNumber == Int32.MaxValue)
                return GetExperimental();

            var answer = new FeatureSet();

            var type = answer.GetType();
            var properties = type.GetRuntimeProperties();

            // Look through all boolean properties
            foreach (var p in properties.Where(i => i.PropertyType == typeof(Boolean)))
            {
                var supportedIn = p.GetCustomAttributes<SupportedInAttribute>().Where(i => i.GetType() == typeof(SupportedInAttribute)).FirstOrDefault();

                // If we're in a build where that exists
                if (supportedIn != null && buildNumber >= supportedIn.MinVersion.Build)
                    p.SetValue(answer, true);

                // If it's device-family specific
                switch (deviceFamily)
                {
                    case DeviceFamily.Mobile:
                        var mobileSupportedIn = p.GetCustomAttribute<MobileSupportedInAttribute>();
                        if (mobileSupportedIn != null && buildNumber >= mobileSupportedIn.MinVersion.Build)
                            p.SetValue(answer, true);
                        break;

                    case DeviceFamily.Desktop:
                        var desktopSupportedIn = p.GetCustomAttribute<DesktopSupportedInAttribute>();
                        if (desktopSupportedIn != null && buildNumber >= desktopSupportedIn.MinVersion.Build)
                            p.SetValue(answer, true);
                        break;
                }
            }

            return answer;
        }

        /// <summary>
        /// Returns feature set with everything true, all features enabled
        /// </summary>
        /// <returns></returns>
        public static FeatureSet GetExperimental()
        {
            var answer = new FeatureSet();

            var type = answer.GetType();
            var properties = type.GetRuntimeProperties();

            // Set all properties to true
            foreach (var p in properties.Where(i => i.PropertyType == typeof(Boolean)))
            {
                p.SetValue(answer, true);
            }

            return answer;
        }
    }
}
