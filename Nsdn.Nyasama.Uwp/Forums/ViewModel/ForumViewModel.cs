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
    public class ForumViewModel
    {
        public ObservableCollection<Forum> ForumIndexes { get; } = new ObservableCollection<Forum>();

        public ForumViewModel() {}

        public async void GetForumIndex()
        {
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "forumindex");
            //获取内容Json
            JObject forumIndexesJObject = await Network.GetContentJsonAsync(rp);
            //将获得的Json结果转换为列表
            IList<JToken> forumIndexesJTokenList = forumIndexesJObject["Variables"]["forumlist"].Children().ToList();
            //将Json结果反序列化为Forum对象
            IList<Forum> searchResults = new List<Forum>();
            foreach (JToken forumIndexJTokenList in forumIndexesJTokenList)
            {
                Forum forumIndex = JsonConvert.DeserializeObject<Forum>(forumIndexJTokenList.ToString());
                //将Forum对象推送到Collection中
                ForumIndexes.Add(forumIndex);
            }

            Regex regex = new Regex(@"data/attachment/[^(jpg)]+jpg");
            foreach (Forum forumIndex in ForumIndexes)
            {
                string match = regex.Match(forumIndex.Icon).Value.ToString();
                forumIndex.Icon = "http://bbs.nyasama.com/" + match;
            }
        }
    }
}
