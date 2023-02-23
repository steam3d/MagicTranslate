// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.Components
{
    public sealed partial class SettingsItemHeader : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(SettingsItemHeader), null);
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkTextProperty =
        DependencyProperty.Register("HyperlinkText", typeof(string), typeof(SettingsItemHeader), null);

        public string HyperlinkText
        {
            get { return (string)GetValue(HyperlinkTextProperty); }
            set { SetValue(HyperlinkTextProperty, value); }
        }

        public event EventHandler HyperlinkClick;
        public SettingsItemHeader()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkClick?.Invoke(this, new EventArgs());
        }
    }
}
