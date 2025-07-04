﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shell.Controls {
    public sealed partial class ActionBarControl : UserControl {
        public GridLength ActionBarItemHeight { get; set; } = new GridLength(48);
        public GridLength ActionBarItemWidth { get; set; } = new GridLength(48);

        public Action OnBack { get; set; }
        public Action OnTaskView { get; set; }
        public Action OnStart { get; set; }
        public Action OnSearch { get; set; }

        public HorizontalAlignment TabletAlignment { get; set; } = HorizontalAlignment.Center;

        public ActionBarControl() {
            this.InitializeComponent();

            this.Control_SizeChanged(null, null);
        }

        private void Control_SizeChanged(Object sender, SizeChangedEventArgs e) {
            if (this.Width > 950)
                switch (this.TabletAlignment) {
                    case HorizontalAlignment.Center:
                        this.LeftPad.Width = new GridLength(1, GridUnitType.Star);
                        this.RightPad.Width = new GridLength(1, GridUnitType.Star);
                        break;
                    case HorizontalAlignment.Left:
                        this.LeftPad.Width = new GridLength(0);
                        this.RightPad.Width = new GridLength(1, GridUnitType.Star);
                        break;
                    case HorizontalAlignment.Right:
                        this.LeftPad.Width = new GridLength(1, GridUnitType.Star);
                        this.RightPad.Width = new GridLength(0);
                        break;
            }

            if (this.Width <= 950) {
            } else {
                if (this.Width >= 1920) {
                    // TODO: Taskbar icons
                }
            }
        }


        private void BackBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.OnBack();
        }

        private void StartBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.OnStart();
        }

        private void SearchBtn_Tapped(Object sender, TappedRoutedEventArgs e) {
            this.OnSearch();
        }

        private void BackBtn_ContextRequested(UIElement sender, ContextRequestedEventArgs args) {
            this.OnTaskView();
        }
    }
}
