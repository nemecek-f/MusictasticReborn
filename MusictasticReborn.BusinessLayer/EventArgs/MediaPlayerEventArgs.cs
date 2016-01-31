using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusictasticReborn.BusinessLayer.Extensions
{
    public class MediaPlayerEventArgs : EventArgs
    {
        public MediaPlayerEventArgs(bool isPlaying, string trackName = "")
        {
            IsPlaying = isPlaying;
            TrackName = trackName;
        }

        public bool IsPlaying { get; set; }

        public string TrackName { get; set; }
    }
}
