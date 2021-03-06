﻿using Newtonsoft.Json;
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
        private bool _hasMorePage = true;
        private IList<JToken> _lastPostsJTokenList;

        public ThreadViewModel()
        {
            _lastPostsJTokenList = new List<JToken>();
            _lastPostsJTokenList.Add("");
        }

        public async Task GetPosts(int tid)
        {
            if (!_hasMorePage)
                return;
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "viewthread");
            rp.Modules.Add("tid", tid.ToString());
            rp.Modules.Add("page", Page.ToString());
            //获取内容Json
            JObject postsJObject = await Network.GetContentJsonAsync(rp);
            //将获得的Json结果转换为列表
            IList<JToken> postsJTokenList = postsJObject["Variables"]["postlist"].Children().ToList();
            if (postsJTokenList.Count == _lastPostsJTokenList.Count && postsJTokenList[0].ToString() == _lastPostsJTokenList[0].ToString())
            {
                _hasMorePage = false;
                return;
            }
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
                Posts.Add(post);
            }
            Page++;
            _lastPostsJTokenList = postsJTokenList;
        }
    }
}
