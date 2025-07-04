using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NotificationsVisualizerLibrary.Model;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary.Controls
{
    public sealed partial class BadgeValueControl : UserControl
    {
        public BadgeValueControl()
        {
            this.InitializeComponent();
        }

        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(BadgeValue), typeof(BadgeValueControl), new PropertyMetadata(BadgeValue.Default()));

        public BadgeValue Value
        {
            get { return this.GetValue(ValueProperty) as BadgeValue; }
            set { this.SetValue(ValueProperty, value); }
        }
    }
}
