using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TimespanExtensions
    {
        public static string ToSongDuration(this TimeSpan ts)
        {
            return String.Format("{0}:{1}", ts.Minutes, ts.Seconds);
        }

        public static string ToPlaylistDuration(this TimeSpan ts)
        {
            return ts.TotalHours > 0 ? String.Format("{0}h {1}m", ts.TotalHours, ts.TotalMinutes) : ToSongDuration(ts);
        }
    }
}
