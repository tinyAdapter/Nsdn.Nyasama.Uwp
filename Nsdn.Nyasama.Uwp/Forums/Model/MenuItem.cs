using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Nsdn.Nyasama.Uwp.Forums.Model
{
    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }

        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Accept, Name = "板块列表", PageType = typeof(ForumPage) });
            items.Add(new MenuItem() { Icon = Symbol.Send, Name = "热门帖子", PageType = typeof(HotThreadPage) });
            items.Add(new MenuItem() { Icon = Symbol.Shop, Name = "汉化推送", PageType = typeof(ForumListPage) });
            return items;
        }

        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Setting, Name = "设定", PageType = typeof(HotThreadPage) });
            items.Add(new MenuItem() { Icon = Symbol.World, Name = "关于", PageType = typeof(HotThreadPage) });
            return items;
        }
    }
}
