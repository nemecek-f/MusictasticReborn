using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer;

namespace MusictasticReborn.ViewModels
{
    public class PlayingNowVm : BindableVm
    {

        public PlayingNowVm()
        {
            MusicPlayerWrapper.Instance.MediaPlayerStateChanged += Instance_MediaPlayerStateChanged;
            MusicPlayerWrapper.Instance.MediaPlayerTrackChanged += Instance_MediaPlayerTrackChanged;
            MusicPlayerWrapper.Instance.AlbumArtChanged += Instance_AlbumArtChanged;
        }

        void Instance_AlbumArtChanged(object sender, EventArgs e)
        {
            AlbumArt = MusicPlayerWrapper.Instance.AlbumArt;
        }

        void Instance_MediaPlayerTrackChanged(object sender, BusinessLayer.Extensions.MediaPlayerEventArgs e)
        {
            CurrentTrackName = e.TrackName;
        }

        void Instance_MediaPlayerStateChanged(object sender, BusinessLayer.Extensions.MediaPlayerEventArgs e)
        {
            if (e.IsPlaying)
            {

            }
            else
            {
                
            }
        }

        public void TogglePlayPause()
        {
            MusicPlayerWrapper.Instance.TogglePlayPause();
        }

        private string _albumArt;
        private string _currentTrackName;

        public string AlbumArt
        {
            get { return _albumArt; }
            set
            {
                _albumArt = value;
                OnPropertyChanged("AlbumArt");
            }
        }

        public string CurrentTrackName
        {
            get { return _currentTrackName; }
            set
            {
                _currentTrackName = value;
                OnPropertyChanged();
            }
        }
    }
}
