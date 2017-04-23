using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nsdn.Nyasama.Uwp.Forums.Model
{
    public class Rating
    {
        public enum Type
        {
            MoePower,
            NyaCoin,
            Point
        }
        public int Score { get; set; }
        public string Username { get; set; }
        public string RatingTime { get; set; }
        public string Reason { get; set; }
    }
}
