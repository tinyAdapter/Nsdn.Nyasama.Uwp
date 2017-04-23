using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nsdn.Nyasama.Uwp.Forums.Model;
using Nsdn.Nyasama.Uwp.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nsdn.Nyasama.Uwp.Forums.ViewModel
{
    public class ForumListViewModel
    {
        public string ThreadTitle { get; set; }
        public ObservableCollection<ThreadHeader> Threads { get; } = new ObservableCollection<ThreadHeader>();
        public int Page { get; set; } = 1;
        private bool _hasMorePage = true;
        private IList<JToken> _lastThreadsJTokenList;

        public ForumListViewModel()
        {
            _lastThreadsJTokenList = new List<JToken>();
            _lastThreadsJTokenList.Add("");
        }

        public async Task GetForumList(int fid)
        {
            if (!_hasMorePage)
                return;
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "forumdisplay");
            rp.Modules.Add("fid", fid.ToString());
            rp.Modules.Add("page", Page.ToString());
            //获取内容Json
            JObject threadsJObject = await Network.GetContentJsonAsync(rp);
            //将获得的Json结果转换为列表
            IList<JToken> threadsJTokenList = threadsJObject["Variables"]["forum_threadlist"].Children().ToList();
            if (threadsJTokenList.Count == _lastThreadsJTokenList.Count && threadsJTokenList[0].ToString() == _lastThreadsJTokenList[0].ToString())
            {
                _hasMorePage = false;
                return;
            }
            foreach (JToken threadJTokenList in threadsJTokenList)
            {
                string s = threadJTokenList.ToString();
                ThreadHeader thread = JsonConvert.DeserializeObject<ThreadHeader>(s);
                //将ThreadHeader对象推送到Collection中
                Threads.Add(thread);
            }
            Page++;
            _lastThreadsJTokenList = threadsJTokenList;
        }
    }
}
