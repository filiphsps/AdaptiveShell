using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NotificationsVisualizerLibrary
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        AppWindow m_AppWindow;

        public MainWindow()
        {
            this.InitializeComponent();

            this.m_AppWindow = this.GetAppWindowForCurrentWindow();

            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = this.m_AppWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                this.AppTitleBar.Loaded += this.AppTitleBar_Loaded;
                this.AppTitleBar.SizeChanged += this.AppTitleBar_SizeChanged;

                this.BackButton.Click += this.OnBackClicked;
                this.BackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Title bar customization using these APIs is currently
                // supported only on Windows 11. In other cases, hide
                // the custom title bar element.
                // AppTitleBar.Visibility = Visibility.Collapsed;
                // TODO Show alternative UI for any functionality in
                // the title bar, such as the back button, if used
            }
        }

        public Button BackButton => this.AppTitleBarBackButton;

        private void AppTitleBar_Loaded(Object sender, RoutedEventArgs e)
        {
            this.SetTitleBar(this.AppTitleBar);
            // TODO Raname MainPage in case your app Main Page has a different name
            this.PageFrame.Navigate(typeof(MainPage));
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                this.SetDragRegionForCustomTitleBar(this.m_AppWindow);
            }
        }

        private void OnBackClicked(Object sender, RoutedEventArgs e)
        {
            if (this.PageFrame.CanGoBack)
            {
                this.PageFrame.GoBack();
            }
        }

        private void AppTitleBar_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            if (AppWindowTitleBar.IsCustomizationSupported()
                && this.m_AppWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                // Update drag region if the size of the title bar changes.
                this.SetDragRegionForCustomTitleBar(this.m_AppWindow);
            }
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }

        private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
        {
            if (AppWindowTitleBar.IsCustomizationSupported()
                && appWindow.TitleBar.ExtendsContentIntoTitleBar)
            {
                Double scaleAdjustment = this.GetScaleAdjustment();

                this.RightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
                this.LeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

                List<Windows.Graphics.RectInt32> dragRectsList = new();

                Windows.Graphics.RectInt32 dragRectL;
                dragRectL.X = (Int32)((this.LeftPaddingColumn.ActualWidth + this.IconColumn.ActualWidth) * scaleAdjustment);
                dragRectL.Y = 0;
                dragRectL.Height = (Int32)((this.AppTitleBar.ActualHeight) * scaleAdjustment);
                dragRectL.Width = (Int32)((this.TitleColumn.ActualWidth
                                        + this.DragColumn.ActualWidth) * scaleAdjustment);
                dragRectsList.Add(dragRectL);

                Windows.Graphics.RectInt32[] dragRects = dragRectsList.ToArray();
                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern Int32 GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out UInt32 dpiX, out UInt32 dpiY);

        internal enum Monitor_DPI_Type : Int32 {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = MDT_Effective_DPI
        }

        private Double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

            // Get DPI.
            Int32 result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out UInt32 dpiX, out UInt32 _);
            if (result != 0)
            {
                throw new Exception("Could not get DPI for monitor.");
            }

            UInt32 scaleFactorPercent = (UInt32)(((Int64)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorPercent / 100.0;
        }
    }
}
