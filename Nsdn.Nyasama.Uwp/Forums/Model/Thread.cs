using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsdn.Nyasama.Uwp.Forums.Model
{
    public class Thread
    {
        public ThreadHeader Header { get; set; }
        public string Body { get; set; }
        public List<Post> Posts { get; set; }
        public DateTime DateLine { get; set; }
    }

    public class ThreadHeader
    {
        public int Tid { get; set; }
        public int Fid { get; set; }
        public string Author { get; set; }
        public int AuthorId { get; set; }
        public UInt64 DbDateLine { get; set; }
        public int Replies { get; set; }
        public UInt64 DbLastPost { get; set; }
        public string LastPoster { get; set; }
        public string Subject { get; set; }
        public int Attachment { get; set; }
        public int Views { get; set; }
        public DateTime DateLine { get; set; }
        public DateTime LastPost { get; set; }
    }
}