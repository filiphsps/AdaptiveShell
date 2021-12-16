using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace Shell.Host {
    struct WindowData {
        public IntPtr hwnd;
        public String title;
    }
    class Functions {
        #region Private variables
        private static WinAPI.BoundingBox m_rcOldDesktopRect;
        private static IntPtr m_hTaskBar;
        #endregion

        public static Double STATUSBAR_HEIGHT { get => /* Features.StatusBarEnabled ? 15 :*/ 0; }
        public static readonly Double ACTIONBAR_HEIGHT = 48;
        public static Double STARTSCREEN_HEIGHT { get => SystemParameters.PrimaryScreenHeight - (Functions.STATUSBAR_HEIGHT + Functions.ACTIONBAR_HEIGHT); }
        public static readonly String SETTINGS_PATH = @$"{Path.GetPathRoot(Environment.SystemDirectory)}.AdaptiveShell\Settings.xml";
        public static readonly String LAYOUT_PATH = @$"{Path.GetPathRoot(Environment.SystemDirectory)}.AdaptiveShell\Layout.xml";

        /// <summary>
        /// Parse settings
        /// </summary>
        public static Shell.Models.SettingsModel LoadSettings() {
            var xmlSerializer = new XmlSerializer(typeof(Shell.Models.SettingsModel));
            Shell.Models.SettingsModel settings;

            try {
                using (var stream = new StreamReader(SETTINGS_PATH)) {
                    settings = (Shell.Models.SettingsModel)xmlSerializer.Deserialize(stream);
                }
            } catch(Exception ex) {
                Debug.WriteLine(ex.Message);

                // TODO: only return new instance if file isn't found
                settings = new Shell.Models.SettingsModel();
            }

            return settings;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        public static Boolean SaveSettings(Shell.Models.SettingsModel settings) {
            try {
                var settingsFile = new FileInfo(SETTINGS_PATH);
                if (!settingsFile.Directory.Exists) System.IO.Directory.CreateDirectory(settingsFile.DirectoryName);

                var xmlSerializer = new XmlSerializer(typeof(Shell.Models.SettingsModel));
                TextWriter stream = new StreamWriter(settingsFile.FullName);

                xmlSerializer.Serialize(stream, settings);
                return true;
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Parse settings
        /// </summary>
        public static Shell.Models.LayoutModel LoadLayout() {
            var xmlSerializer = new XmlSerializer(typeof(Shell.Models.LayoutModel));
            Shell.Models.LayoutModel layout;

            try {
                using (var stream = new StreamReader(LAYOUT_PATH)) {
                    layout = (Shell.Models.LayoutModel)xmlSerializer.Deserialize(stream);
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);

                // TODO: only return new instance if file isn't found
                layout = new Shell.Models.LayoutModel();
            }

            return layout;
        }

        public static void DisableWPFTabletSupport() {
            // Get a collection of the tablet devices for this window.
            TabletDeviceCollection devices = System.Windows.Input.Tablet.TabletDevices;

            if (devices.Count > 0) {
                // Get the Type of InputManager.  
                Type inputManagerType = typeof(System.Windows.Input.InputManager);

                // Call the StylusLogic method on the InputManager.Current instance.  
                Object stylusLogic = inputManagerType.InvokeMember("StylusLogic",
                            BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                            null, InputManager.Current, null);

                if (stylusLogic != null) {
                    //  Get the type of the stylusLogic returned from the call to StylusLogic.
                    Type stylusLogicType = stylusLogic.GetType();

                    // Loop until there are no more devices to remove.
                    while (devices.Count > 0) {
                        // Remove the first tablet device in the devices collection.
                        stylusLogicType.InvokeMember("OnTabletRemoved",
                                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                                null, stylusLogic, new Object[] { (UInt32)0 });
                    }
                }

            }
        }

        /// <summary>
        /// Resizes the Desktop area to our shells' requirements
        /// </summary>
        public static void MakeNewDesktopArea() {
            // Save current Working Area size
            m_rcOldDesktopRect.left = (Int32)SystemParameters.WorkArea.Left;
            m_rcOldDesktopRect.top = (Int32)SystemParameters.WorkArea.Top;
            m_rcOldDesktopRect.right = (Int32)SystemParameters.WorkArea.Right;
            m_rcOldDesktopRect.bottom = (Int32)SystemParameters.WorkArea.Bottom;

            // Make a new Workspace
            WinAPI.BoundingBox rc;
            rc.left = 0;
            rc.top = (Int32)(STATUSBAR_HEIGHT); // statusbar
            rc.right = (Int32)SystemParameters.WorkArea.Right;
            rc.bottom = (Int32)(SystemParameters.WorkArea.Top - ACTIONBAR_HEIGHT); // actionbar/taskbar
            WinAPI.SystemParametersInfo((Int32)WinAPI.SPI.SPI_SETWORKAREA, 0, ref rc, 0);
        }

        /// <summary>
        /// Restores the Desktop area
        /// </summary>
        public static void RestoreDesktopArea() {
            WinAPI.SystemParametersInfo((Int32)WinAPI.SPI.SPI_SETWORKAREA, 0, ref m_rcOldDesktopRect, 0);
        }

        /// <summary>
        /// Hides the Windows Taskbar
        /// </summary>
        public static Boolean HideTaskBar() {
            // Get the Handle to the Windows Taskbar
            m_hTaskBar = WinAPI.FindWindow("Shell_TrayWnd", null);

            if (m_hTaskBar == null || m_hTaskBar == IntPtr.Zero) return false;

            // Hide the Taskbar
            WinAPI.ShowWindow(m_hTaskBar, (Int32)WinAPI.WindowShowStyle.Hide);
            return true;
        }

        /// <summary>
        /// Show the Windows Taskbar
        /// </summary>
        public static Boolean ShowTaskBar() {
            if (m_hTaskBar == null || m_hTaskBar == IntPtr.Zero) return false;

            WinAPI.ShowWindow(m_hTaskBar, (Int32)WinAPI.WindowShowStyle.Show);
            return true;
        }

        /// <summary>
        /// Gets a list of Active Tasks
        /// </summary>
        public static ArrayList GetActiveTasks() {
            var ar = new ArrayList();
            IntPtr child = IntPtr.Zero;

            Process[] process = Process.GetProcesses();
            foreach (Process p in process) {
                WindowData w;
                if (p.MainWindowHandle != IntPtr.Zero && p.MainWindowTitle.Length > 0) {
                    w.hwnd = p.MainWindowHandle;
                    w.title = p.MainWindowTitle;
                    ar.Add(w);
                }
            }
            return ar;
        }
    }
}
