using AdaptiveShell.LiveTiles.Models;
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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.Controls {
    public sealed partial class BadgeValueControl : UserControl {
        public BadgeValueControl() {
            this.InitializeComponent();
        }

        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(BadgeValue), typeof(BadgeValueControl), new PropertyMetadata(BadgeValue.Default()));

        public BadgeValue Value {
            get => this.GetValue(ValueProperty) as BadgeValue;
            set => this.SetValue(ValueProperty, value);
        }
    }
}
