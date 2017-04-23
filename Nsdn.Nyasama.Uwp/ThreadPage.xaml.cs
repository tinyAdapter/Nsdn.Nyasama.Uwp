using Nsdn.Nyasama.Uwp.Forums.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Nsdn.Nyasama.Uwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ThreadPage : Page
    {
        public ThreadViewModel ViewModel;
        private int _pid { get; set; }

        public ThreadPage()
        {
            this.InitializeComponent();
            this.ViewModel = new ThreadViewModel();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _pid = Int32.Parse(e.Parameter != null ? e.Parameter.ToString() : "10000");
            ViewModel.GetPosts(_pid);
        }

        private async void ContentWebView_WidthChanged(object sender, SizeChangedEventArgs e)
        {
            var webView = sender as WebView;

            // get the total width and height
            var heightString = await webView.InvokeScriptAsync("eval", new[] { "(document.body.clientHeight + 8).toString()" });
            await webView.InvokeScriptAsync("eval", new string[] { "document.body.style.overflow = 'hidden';" });

            if (!int.TryParse(heightString, out int height))
            {
                throw new Exception("Unable to get page height");
            }
            // resize the webview to the content
            webView.Height = height;
        }

        private async void ContentWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var webView = sender as WebView;

            // get the total width and height
            var heightString = await webView.InvokeScriptAsync("eval", new[] { "(document.body.clientHeight + 8).toString()" });
            await webView.InvokeScriptAsync("eval", new string[] { "document.body.style.overflow = 'hidden';" });

            if (!int.TryParse(heightString, out int height))
            {
                throw new Exception("Unable to get page height");
            }
            // resize the webview to the content
            webView.Height = height;
        }
    }

    public class Untils
    {
        public static readonly DependencyProperty SourceStringProperty = DependencyProperty.RegisterAttached("SourceString", typeof(string), typeof(Untils), new PropertyMetadata("", OnSourceStringChanged));

        public static string GetSourceString(DependencyObject obj)
        {
            return obj.GetValue(SourceStringProperty).ToString();
        }

        public static void SetSourceString(DependencyObject obj, string value)
        {
            obj.SetValue(SourceStringProperty, value);
        }

        public static void OnSourceStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView wv = d as WebView;
            if (wv != null)
            {
                wv.NavigateToString(e.NewValue.ToString());
            }
        }
    }
}