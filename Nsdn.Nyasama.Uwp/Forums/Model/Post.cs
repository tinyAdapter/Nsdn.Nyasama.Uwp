using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsdn.Nyasama.Uwp.Forums.Model
{
    public class Post
    {
        public int Pid { get; set; }
        public string Author { get; set; }
        public int AuthorId { get; set; }
        public string DateLine { get; set; }
        public string Message { get; set; }
        public int Anonymous { get; set; }
        public int Status { get; set; }
        public string AuthorAvaterLink { get; set; }
    }
}
