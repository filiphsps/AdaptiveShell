using NotificationsVisualizerLibrary.Converters;
using NotificationsVisualizerLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NotificationsVisualizerLibrary.Controls
{
    internal static class AdaptiveGenerator_Progress
    {
        public static FrameworkElement Generate(AdaptiveProgress progress)
        {
            return new ProgressView()
            {
                DataContext = progress
            };
        }
    }
}
