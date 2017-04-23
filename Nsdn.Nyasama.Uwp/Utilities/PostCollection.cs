using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nsdn.Nyasama.Uwp.Forums.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Nsdn.Nyasama.Uwp.Utilities
{
    public class PostCollection : ObservableCollection<Post>, ISupportIncrementalLoading
    {
        private int _loadedPage = 0;
        private int _tid;

        public PostCollection(int tid)
        {
            _tid = tid;
            GetPosts(1);
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return AsyncInfo.Run((c) => GetPosts(_loadedPage + 1));
        }

        public bool HasMoreItems{ get { return HasMorePosts().Result; } }

        public async Task<LoadMoreItemsResult> GetPosts(int page)
        {
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "viewthread");
            rp.Modules.Add("tid", _tid.ToString());
            rp.Modules.Add("page", page.ToString());
            //获取内容Json
            JObject postsJObject = await Network.GetContentJsonAsync(rp);
            //将获得的Json结果转换为列表
            IList<JToken> postsJTokenList = postsJObject["Variables"]["postlist"].Children().ToList();
            foreach (JToken postJTokenList in postsJTokenList)
            {
                Post post = JsonConvert.DeserializeObject<Post>(postJTokenList.ToString());
                //删除blockquote"引用"文字
                Regex regexQuote = new Regex("<blockquote><p>引用:</p>", RegexOptions.IgnoreCase);
                post.Message = regexQuote.Replace(post.Message, (match) =>
                {
                    return match.Value.Replace("引用:", "");
                });
                //绑定图片div
                //Regex regexImage = new Regex("<img src=\\\\\\\"[^(static)]+[^g]*\\.png|<img src=\\\\\\\"[^(static)]+[^g]*\\.jpg", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex regexImage = new Regex("<img src=\\s*\\\"http", RegexOptions.IgnoreCase);
                post.Message = regexImage.Replace(post.Message, (match) =>
                {
                    return match.Value.Replace("<img src=", "<img class=\"resize\" src=");
                });
                //修复表情图HTML格式
                Regex regexFace = new Regex(@"static/image/smiley/[^f]*\.gif|static/image/smiley/[^g]*\.jpg", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                post.Message = regexFace.Replace(post.Message, (match) =>
                {
                    return Network.NYASAMA_URL + match.Value;
                });
                //修改Message，使之符合WebView格式
                post.Message = "<!DOCTYPE html><html><head><style>.resize{width:100%;height:auto;}.quote{padding:8px;background:#F9F9F9 url(http://bbs.nyasama.com/static/image/common/icon_quote_s.gif) no-repeat 20px 6px;}.pl .quote blockquote{display:inline-block;margin:0;padding:8px 8px 15px 8px;background:url(http://bbs.nyasama.com/static/image/common/icon_quote_e.gif) no-repeat 100% 100%;line-height:1.6;zoom:1;}</style></head><body>" + post.Message + "</body></html>";
                //获取用户头像链接
                post.AuthorAvaterLink = Network.GetUserAvatarLink(post.AuthorId);
                //将Post对象推送到Collection中
                Items.Add(post);
            }
            _loadedPage++;
            return new LoadMoreItemsResult { Count = (uint)postsJTokenList.Count };
        }

        public async Task<bool> HasMorePosts()
        {
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "viewthread");
            rp.Modules.Add("tid", _tid.ToString());
            rp.Modules.Add("page", (_loadedPage + 1).ToString());
            //获取内容Json
            JObject postsJObject = await Network.GetContentJsonAsync(rp);
            //判断是否有未加载页
            if (postsJObject["Variables"]["postlist"].Children().ToList() == null)
                return false;
            return true;
        }
    }
}
