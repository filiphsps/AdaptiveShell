using AdaptiveShell.ViewModels.SettingsViewModels;
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

namespace AdaptiveShell.Views.SettingsPages {
    public sealed partial class About : Page {
        public AboutViewModel ViewModel {
            get => (AboutViewModel)this.DataContext;
            set => this.DataContext = value;
        }

        public About() {
            this.InitializeComponent();

            this.ViewModel = new AboutViewModel();
        }
    }
}
