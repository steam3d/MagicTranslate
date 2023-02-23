using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace MagicTranslate.UI.Components
{
    public sealed partial class SettingsItem : UserControl
    {
        public static readonly DependencyProperty SettingsContentProperty =
        DependencyProperty.Register("SettingsContent", typeof(object), typeof(SettingsItem), null);
        public object SettingsContent
        {
            get { return (object)GetValue(SettingsContentProperty); }
            set { SetValue(SettingsContentProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(SettingsItem), null);
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(SettingsItem), null);

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty HyperlinkTextProperty =
        DependencyProperty.Register("HyperlinkText", typeof(string), typeof(SettingsItem), null);

        public string HyperlinkText
        {
            get { return (string)GetValue(HyperlinkTextProperty); }
            set { SetValue(HyperlinkTextProperty, value); }
        }

        public event EventHandler HyperlinkClick;

        public SettingsItem()
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
