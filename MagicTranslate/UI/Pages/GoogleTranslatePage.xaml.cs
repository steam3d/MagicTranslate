// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using MagicTranslate.Args;
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
using System.Text.Json;
using TranslateLibrary;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MagicTranslate.UI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GoogleTranslatePage : Page
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public GoogleTranslateResult TranslateResult { get; set; }

        public GoogleTranslatePage()
        {
            this.InitializeComponent();
            DataContext = this;            
            //string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json.json"));
            //Translate translate = new Translate();
            //TranslateResult = translate.TranslateGoogle("Ручка", "ru", "en");
            //TranslateResult = new GoogleTranslateResult(JsonSerializer.Deserialize<GoogleTranslateJson>(json),null,null,TimeSpan.Zero,null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Tutorial
            if (e.Parameter == null)
            {
                string json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json.json"));
                TranslateResult = new GoogleTranslateResult(JsonSerializer.Deserialize<GoogleTranslateJson>(json), null, null, TimeSpan.Zero, null);
                return;
            }

            Translate translate = new Translate();
            var args = e.Parameter as TranslateArgs;
            TranslateResult = translate.TranslateGoogle(args.TextToTranslate, args.TranslateFrom.TwoLetterISOLanguageName, args.TranslateTo.TwoLetterISOLanguageName);
            base.OnNavigatedTo(e);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Control control)
            {
                var url = control.Tag as string;
                if (!string.IsNullOrEmpty(url))
                {
                    await Windows.System.Launcher.LaunchUriAsync(new System.Uri(url));
                }
            }
        }
    }
}
