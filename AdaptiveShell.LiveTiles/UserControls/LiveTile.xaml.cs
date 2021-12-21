﻿using AdaptiveShell.LiveTiles.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AdaptiveShell.LiveTiles.UserControls {
    public sealed partial class LiveTile : UserControl {
        public String AppId {
            get => (String)this.GetValue(AppIdProperty);
            set => this.SetValue(AppIdProperty, value);
        }

        public static readonly DependencyProperty AppIdProperty = DependencyProperty.Register("AppId",
            typeof(String), typeof(LiveTile), null);

        public LiveTile() {
            this.InitializeComponent();
        }
    }
}
