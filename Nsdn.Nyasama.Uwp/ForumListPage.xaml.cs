using Nsdn.Nyasama.Uwp.Forums.Model;
using Nsdn.Nyasama.Uwp.Forums.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class ForumListPage : Page
    {
        public ForumListViewModel ViewModel;
        private int _fid;
        private double _originHeight;
        private bool _isLoading;

        public ForumListPage()
        {
            this.InitializeComponent();
            ViewModel = new ForumListViewModel();
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _fid = Int32.Parse(e.Parameter != null ? e.Parameter.ToString() : "144");
            await ViewModel.GetForumList(_fid);
        }

        private void ForumListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ThreadHeader;
            Frame.Navigate(typeof(ThreadPage), item.Tid);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (RootScrollViewer.VerticalOffset == _originHeight) return;
            _originHeight = RootScrollViewer.VerticalOffset;

            if (_isLoading) return;
            if (RootScrollViewer.VerticalOffset <= RootScrollViewer.ScrollableHeight - 200) return;

            _isLoading = true;
            await Task.Factory.StartNew(async () =>
            {
                //调用UI线程添加数据
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // 拼接业务查询URL
                    await ViewModel.GetForumList(_fid);
                    _isLoading = false;
                });
            });
        }
    }
}
