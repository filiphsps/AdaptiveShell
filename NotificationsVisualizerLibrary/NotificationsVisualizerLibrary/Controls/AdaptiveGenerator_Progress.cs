using Microsoft.UI.Xaml;
using NotificationsVisualizerLibrary.Model;

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
