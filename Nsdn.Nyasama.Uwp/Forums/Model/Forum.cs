using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsdn.Nyasama.Uwp.Forums.Model
{
    public class Forum
    {
        public int Fid { get; set; }
        public string Name { get; set; }
        public int Threads { get; set; }
        public int Posts { get; set; }
        public int TodayPosts { get; set; }
        public string Description { get; set; }
        public String Icon { get; set; }
    }
}
