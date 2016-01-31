using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Playback;
using System.Collections.Specialized;

namespace MusictasticReborn.Shared
{
    public class QueueManager
    {
        public static Queue CreateNewPlaylist(IEnumerable<LightSongModel> songs)
        {
            return new Queue(songs);
        }

        public sealed class Queue : IDisposable
        {
            private readonly List<LightSongModel> _songs;

            int _currentTrackId = -1;
            private readonly MediaPlayer _mediaPlayer;
            private TimeSpan _startPosition = TimeSpan.FromSeconds(0);
            
            public LightSongModel CurrentTrack
            {
                get
                {
                    if (_currentTrackId == -1)
                    {
                        return LightSongModel.Null;
                    }
                    if (_currentTrackId < _songs.Count)
                    {
                        return _songs[_currentTrackId];
                    }
                    else
                        throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
                }
            }

            internal Queue(IEnumerable<LightSongModel> songs)
            {

                _songs = new List<LightSongModel>(songs);
                _mediaPlayer = BackgroundMediaPlayer.Current;

                _mediaPlayer.MediaOpened += _mediaPlayer_MediaOpened;
                _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;

                _mediaPlayer.CurrentStateChanged += _mediaPlayer_CurrentStateChanged;

                _mediaPlayer.MediaFailed += _mediaPlayer_MediaFailed;
            }

           
            public void UpdateSongs(IEnumerable<LightSongModel> songs)
            {
                _songs.Clear();
                _songs.AddRange(songs);
            }

            public event TypedEventHandler<Queue, object> TrackChanged;

            void _mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
            {
                SkipToNext();
            }

            private void _mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
            {
                if (sender.CurrentState == MediaPlayerState.Playing && _startPosition != TimeSpan.FromSeconds(0))
                {
                    sender.Position = _startPosition;
                    sender.Volume = 1.0;
                    _startPosition = TimeSpan.FromSeconds(0);
                    sender.PlaybackMediaMarkers.Clear();
                }
            }

            void _mediaPlayer_MediaEnded(MediaPlayer sender, object args)
            {
                SkipToNext();
            }

            void _mediaPlayer_MediaOpened(MediaPlayer sender, object args)
            {
                sender.Play();
                
                TrackChanged?.Invoke(this, CurrentTrack);
            }

            public void StartPlaying()
            {
                StartTrackAt(0);
                Debug.WriteLine("Playlist starting playing from #0");

            }

            public void SkipToNext()
            {
                StartTrackAt((_currentTrackId + 1) % _songs.Count);
                
                TrackChanged?.Invoke(this, CurrentTrack);
            }

            public void SkipToPrevious()
            {
                if (_currentTrackId == 0)
                {
                    StartTrackAt(_currentTrackId);
                }
                else
                {
                    StartTrackAt(_currentTrackId - 1);
                }
                
                TrackChanged?.Invoke(this, CurrentTrack);
            }

            private void StartTrackAt(int id)
            {
                string source = _songs[id].Path;
                _currentTrackId = id;
                _mediaPlayer.AutoPlay = false;
                _mediaPlayer.SetUriSource(new Uri(source));
            }

            public void StartTrackAt(string name)
            {
                StartTrackAt(name, TimeSpan.FromSeconds(0));
            }

            public void StartTrackAt(string name, TimeSpan position)
            {
                var song = _songs.Find(s => s.Name.Equals(name));
                if (song != null)
                {
                    _currentTrackId = _songs.IndexOf(song);
                    _mediaPlayer.AutoPlay = false;
                    _mediaPlayer.Position = position;
                    _mediaPlayer.SetUriSource(new Uri(song.Path));
                }
            }
            
            public void Dispose()
            {
                _mediaPlayer.MediaOpened -= _mediaPlayer_MediaOpened;
                _mediaPlayer.MediaEnded -= _mediaPlayer_MediaEnded;
                _mediaPlayer.CurrentStateChanged -= _mediaPlayer_CurrentStateChanged;

                _mediaPlayer.MediaFailed -= _mediaPlayer_MediaFailed;
            }
        }
    }
}
