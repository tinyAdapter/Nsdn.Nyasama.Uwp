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
    public class ThreadViewModel
    {
        public string ThreadTitle { get; set; }
        public ObservableCollection<Post> Posts { get; } = new ObservableCollection<Post>();

        public int Page { get; set; } = 1;

        public async void GetPosts(int tid)
        {
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "viewthread");
            rp.Modules.Add("tid", tid.ToString());
            rp.Modules.Add("page", Page.ToString());
            //获取内容Json
            JObject postsJObject = await Network.GetContentJsonAsync(rp);
            //将获得的Json结果转换为列表
            IList<JToken> postsJTokenList = postsJObject["Variables"]["postlist"].Children().ToList();
            //将Json结果反序列化为Post对象
            IList<Post> searchResults = new List<Post>();
            foreach (JToken postJTokenList in postsJTokenList)
            {
                Post post = JsonConvert.DeserializeObject<Post>(postJTokenList.ToString());
                //修复表情图HTML格式
                Regex regex = new Regex(@"static/image/smiley/[^f]*\.gif|static/image/smiley/[^g]*\.jpg", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                post.Message = regex.Replace(post.Message, (match) =>
                 {
                     return Network.NYASAMA_URL + match.Value;
                 });
                //修改Message，使之符合WebView格式
                post.Message = $"<html><head><style>.quote{{padding-bottom:5px;background:#F9F9F9 url(http://bbs.nyasama.com/static/image/common/icon_quote_s.gif) no-repeat 20px 6px;}}.pl .quote blockquote{{display:inline-block;margin:0;padding:0 65px 5px 0;background:url(http://bbs.nyasama.com/static/image/common/icon_quote_e.gif) no-repeat 100% 100%;line-height:1.6;zoom:1;}}</style></head><body>{post.Message}</body></html>";
                //获取用户头像链接
                post.AuthorAvaterLink = Network.GetUserAvatarLink(post.AuthorId);
                //将Post对象推送到Collection中
                Posts.Add(post);
            }
        }
    }
}
