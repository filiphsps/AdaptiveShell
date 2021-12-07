using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace NotificationsVisualizerLibrary.Helpers
{
    /// <summary>
    /// A helper class for obtaining device information.
    /// </summary>
    public static class DeviceInfoHelper
    {
        private static int? _currentOSBuildNumber;
        /// <summary>
        /// Dynamically gets the current OS version
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentOSBuildNumber()
        {
            if (_currentOSBuildNumber == null)
            {
                // The DeviceFamilyVersion is a string, which is actually a ulong number representing the version
                // https://www.suchan.cz/2015/08/uwp-quick-tip-getting-device-os-and-app-info/

                ulong versionAsLong = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);

                //ulong v1 = (versionAsLong & 0xFFFF000000000000L) >> 48;
                //ulong v2 = (versionAsLong & 0x0000FFFF00000000L) >> 32;

                // Build
                ulong v3 = (versionAsLong & 0x00000000FFFF0000L) >> 16;

                //ulong v4 = (versionAsLong & 0x000000000000FFFFL);

                _currentOSBuildNumber = (int)v3;
            }

            return _currentOSBuildNumber.Value;
        }
    }
}
