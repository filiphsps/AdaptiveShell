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
        private const int R_19H2 = 19042;
        private const int R_19H1 = 19041;
        private const int RS4 = 17000;
        private const int RS3 = 16299;
        private const int RS2 = 15063;
        private const int RS1 = 14393;
        private const int TH2 = 10586;
        private const int TH1 = 10240;

        private class SupportedInAttribute : Attribute
        {
            public Version MinVersion { get; private set; }

            public SupportedInAttribute(int minBuildNumber)
            {
                MinVersion = new Version(10, 0, minBuildNumber, 0);
            }
        }

        private class MobileSupportedInAttribute : SupportedInAttribute
        {
            public MobileSupportedInAttribute(int minBuildNumber) : base(minBuildNumber) { }
        }

        private class DesktopSupportedInAttribute : SupportedInAttribute
        {
            public DesktopSupportedInAttribute(int minBuildNumber) : base(minBuildNumber) { }
        }

        public static readonly Version CURRENT_RELEASED_OS_VERSION = new Version(10, 0, R_19H2, 0);

        /// <summary>
        /// Dynamically gets the current OS version
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentOSBuildNumber()
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
        public bool R_19H2_Style_Toasts { get; private set; }

        [SupportedIn(R_19H1)]
        public bool R_19H1_Style_Toasts { get; private set; }

        [SupportedIn(RS3)]
        public bool RS3_Style_Toasts { get; private set; }

        [SupportedIn(RS2)]
        public bool ToastDisplayTimestamp { get; private set; }

        [SupportedIn(RS2)]
        public bool ToastHeaders { get; private set; }

        [DesktopSupportedIn(RS2)]
        public bool ToastProgressBar { get; private set; }

        [SupportedIn(RS2)]
        public bool ToastTextDataBinding { get; private set; }

        [SupportedIn(RS1)]
        public bool ToastContextMenu { get; private set; }

        [SupportedIn(RS1)]
        public bool ToastAttribution { get; private set; }

        [SupportedIn(RS1)]
        public bool AdaptiveToasts { get; private set; }

        [SupportedIn(RS1)]
        public bool RS1_Style_Toasts { get; private set; }

        [SupportedIn(RS1)]
        public bool ChaseableTiles { get; private set; }

        [SupportedIn(TH2)]
        public bool BackgroundAndPeekImage { get; private set; }
        
        [SupportedIn(TH2)]
        public bool CropCircleOnPeekImage { get; private set; }
        
        [SupportedIn(TH2)]
        public bool CropCircleOnBackgroundImage { get; private set; }
        
        [MobileSupportedIn(TH1)]
        [SupportedIn(TH2)]
        public bool SpecialTemplatePeople { get; private set; }

        /// <summary>
        /// Bug 3749658
        /// </summary>
        [SupportedIn(TH2)]
        public bool CircleImageVerticalStretch { get; private set; }

        /// <summary>
        /// Bug 3159429 (overlay on binding applies to both background and peek, AND can be specified individually on each image element too)
        /// </summary>
        [SupportedIn(TH2)]
        public bool OverlayForBothBackgroundAndPeek { get; private set; }

        /// <summary>
        /// Bug 3660904 (Tile payloads that only have a background image should still respect the hint-overlay property (no longer require the tile to have text before the overlay gets applied)
        /// </summary>
        [SupportedIn(TH2)]
        public bool RespectDevSpecifiedBackgroundOverlayEvenWhenNoTextOnTile { get; private set; }



        public static FeatureSet Get(DeviceFamily deviceFamily, int buildNumber)
        {
            if (buildNumber == int.MaxValue)
                return GetExperimental();

            FeatureSet answer = new FeatureSet();

            var type = answer.GetType();
            var properties = type.GetRuntimeProperties();

            // Look through all boolean properties
            foreach (var p in properties.Where(i => i.PropertyType == typeof(bool)))
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
            FeatureSet answer = new FeatureSet();

            var type = answer.GetType();
            var properties = type.GetRuntimeProperties();

            // Set all properties to true
            foreach (var p in properties.Where(i => i.PropertyType == typeof(bool)))
            {
                p.SetValue(answer, true);
            }

            return answer;
        }
    }
}
