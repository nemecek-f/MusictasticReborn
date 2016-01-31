using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows.Media.Playback
{
    public static class BackgroundMediaPlayerExtensions
    {
        public static bool IsPlayingOrPaused(this MediaPlayerState state)
        {
            return state == MediaPlayerState.Playing || state == MediaPlayerState.Paused;
        }
    }
}
