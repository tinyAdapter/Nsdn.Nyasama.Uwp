using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using Nsdn.Nyasama.Uwp.Forums.Model;
using Nsdn.Nyasama.Uwp.Utilities;

namespace Nsdn.Nyasama.Uwp.Forums.ViewModel
{
    public class HotThreadViewModel
    {
        public ObservableCollection<ThreadHeader> HotThreads { get; } = new ObservableCollection<ThreadHeader>();

        public HotThreadViewModel()
        {
            GetHotThreads();
        }

        public async void GetHotThreads()
        {
            //设置HTTP请求参数
            RequestParams rp = new RequestParams();
            rp.Modules.Add("module", "hotthread");
            //获取内容Json
            JObject hotThreadsJObject = await Network.GetContentJsonAsync(rp);
            //将获得的Json结果转换为列表
            IList<JToken> hotThreadsJTokenList = hotThreadsJObject["Variables"]["data"].Children().ToList();
            foreach (JToken hotThreadJTokenList in hotThreadsJTokenList)
            {
                string s = hotThreadJTokenList.ToString();
                ThreadHeader hotThread = JsonConvert.DeserializeObject<ThreadHeader>(s);
                //将ThreadHeader对象推送到Collection中
                HotThreads.Add(hotThread);
            }
        }
    }
}