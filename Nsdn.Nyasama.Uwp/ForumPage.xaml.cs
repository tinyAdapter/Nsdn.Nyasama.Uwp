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
    public sealed partial class ForumPage : Page
    {
        public ForumViewModel ViewModel;

        public ForumPage()
        {
            this.InitializeComponent();
            Utilities.Transitions.SetUpPageAnimation(this);
            this.ViewModel = new ForumViewModel();
            //获取板块列表
            ViewModel.GetForumIndex();
        }

        private void ForumIndexListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Forum;
            Frame.Navigate(typeof(ForumListPage), item.Fid);
        }
    }
}
