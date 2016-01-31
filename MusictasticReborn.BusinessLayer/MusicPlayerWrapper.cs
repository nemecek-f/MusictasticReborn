using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using MusictasticReborn.BusinessLayer.Cache;
using MusictasticReborn.BusinessLayer.Extensions;
using MusictasticReborn.BusinessLayer.Helpers;
using MusictasticReborn.BusinessLayer.Interfaces;
using MusictasticReborn.BusinessLayer.Models;
using MusictasticReborn.Shared;

namespace MusictasticReborn.BusinessLayer
{
    public sealed class MusicPlayerWrapper
    {
        private static readonly MusicPlayerWrapper _instance = new MusicPlayerWrapper();

        public static MusicPlayerWrapper Instance => _instance;

        public event EventHandler<MediaPlayerEventArgs> MediaPlayerStateChanged;

        public event EventHandler<MediaPlayerEventArgs> MediaPlayerTrackChanged;

        public event EventHandler AlbumArtChanged;

        private AutoResetEvent senderInitialized;

        private QueueManager.Queue CurrentQueue;

        public CoreDispatcher Dispatcher { get; set; }

        private AlbumArtCache _albumArtCache;

        private MusicPlayerWrapper()
        {
            senderInitialized = new AutoResetEvent(false);

            _albumArtCache = new AlbumArtCache();
        }

        private bool _isBackgroundTaskRunning;

        public string AlbumArt { get; set; }

        private void OnMediaPlayerStateChanged(bool isPlaying)
        {
            MediaPlayerStateChanged?.Invoke(this, new MediaPlayerEventArgs(isPlaying));
        }

        private void OnMediaPlayerTrackChanged(string trackName)
        {
            MediaPlayerTrackChanged?.Invoke(this, new MediaPlayerEventArgs(false, trackName));
        }

        private void OnAlbumArtChanged()
        {
            AlbumArtChanged?.Invoke(this, new EventArgs());
        }

        public void SaveActiveState()
        {
            ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.AppState, ConstantValues.ForegroundAppActive);
        }
        
        private bool IsMyBackgroundTaskRunning
        {
            get
            {
                if (_isBackgroundTaskRunning)
                    return true;

                object value = ApplicationSettingsHelper.ReadResetSettingsValue(ConstantValues.BackgroundTaskState);
                if (value == null)
                {
                    return false;
                }
                else
                {
                    _isBackgroundTaskRunning = ((String)value).Equals(ConstantValues.BackgroundTaskRunning);
                    return _isBackgroundTaskRunning;
                }
            }
        }

        public void AppSuspending()
        {
            ValueSet messageDictionary = new ValueSet();
            messageDictionary.Add(ConstantValues.AppSuspended, DateTime.Now.ToString());
            BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);
            RemoveMediaPlayerEventHandlers();
            ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.AppState, ConstantValues.ForegroundAppSuspended);
        }

        public void AppResuming()
        {
            ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.AppState, ConstantValues.ForegroundAppActive);

            if (IsMyBackgroundTaskRunning)
            {

                AddMediaPlayerEventHandlers();

                ValueSet messageDictionary = new ValueSet();
                messageDictionary.Add(ConstantValues.AppResumed, DateTime.Now.ToString());
                BackgroundMediaPlayer.SendMessageToBackground(messageDictionary);

                if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                {

                }
                else
                {

                }

            }
            else
            {

            }
        }

        private void AddMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        private void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key)
                {
                    case ConstantValues.Trackchanged:
                        OnMediaPlayerTrackChanged((string)e.Data[key]);
                        break;
                    case ConstantValues.BackgroundTaskStarted:

                        senderInitialized.Set();
                        break;
                }
            }
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            OnMediaPlayerStateChanged(isPlaying: sender.CurrentState == MediaPlayerState.Playing);
        }

        private void RemoveMediaPlayerEventHandlers()
        {
            BackgroundMediaPlayer.Current.CurrentStateChanged -= MediaPlayer_CurrentStateChanged;
            BackgroundMediaPlayer.MessageReceivedFromBackground -= BackgroundMediaPlayer_MessageReceivedFromBackground;
        }

        public void BuildArtCache(IEnumerable<AlbumModel> albums)
        {
            _albumArtCache.BuildCache(albums);
        }

        public void Play(IPlayable music)
        {
            var songs = music.GetSongs().ToList();

            if (songs.Count == 1)
            {
                // load other songs from album
            }
            else if (songs.Count == 0)
            {

            }

            StartPlaying(songs.Select(s => new LightSongModel(s.Path, s.Name, s.Artist)));

            AlbumArt = _albumArtCache.GetAlbumArt(songs.First().AlbumId);

            OnAlbumArtChanged();

        }

        private void StartPlaying(IEnumerable<LightSongModel> songs)
        {
            ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.CurrentPlaylist,
                SongsSerializer.Serialize(songs));


            if (IsMyBackgroundTaskRunning)
            {
                if (MediaPlayerState.Closed == BackgroundMediaPlayer.Current.CurrentState)
                {
                    StartBackgroundAudioTask();

                    var message = new ValueSet { { ConstantValues.StartPlayback, "0" } };

                    SendMessageToBackgroundPlayer(message);
                }
                else
                {
                    SendMessageToBackgroundPlayer(new ValueSet() { { ConstantValues.RefreshPlaylist, "true" } });
                }
            }
            else
            {
                StartBackgroundAudioTask();
            }
        }

        public void TogglePlayPause()
        {
            if (MediaPlayerState.Playing == BackgroundMediaPlayer.Current.CurrentState)
            {
                BackgroundMediaPlayer.Current.Pause();
            }
            else if (MediaPlayerState.Paused == BackgroundMediaPlayer.Current.CurrentState)
            {
                BackgroundMediaPlayer.Current.Play();
            }
        }

        public void Skip()
        {
            if (!BackgroundMediaPlayer.Current.CurrentState.IsPlayingOrPaused())
                return;

            var message = new ValueSet { { ConstantValues.SkipPrevious, "0" } };

            SendMessageToBackgroundPlayer(message);
        }

        public void Next()
        {
            if (!BackgroundMediaPlayer.Current.CurrentState.IsPlayingOrPaused())
                return;

            var message = new ValueSet { { ConstantValues.SkipNext, "0" } };

            SendMessageToBackgroundPlayer(message);
        }

        private void StartBackgroundAudioTask()
        {
            AddMediaPlayerEventHandlers();

            var backgroundtaskinitializationresult = Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                bool initializationSuccesfull = senderInitialized.WaitOne(3000);
                
                if (initializationSuccesfull)
                {
                    var message = new ValueSet {{ConstantValues.StartPlayback, "0"}};

                    SendMessageToBackgroundPlayer(message);

                    SendMessageToBackgroundPlayer(message);
                }
            });

            backgroundtaskinitializationresult.Completed = new AsyncActionCompletedHandler(BackgroundTaskInitializationCompleted);
        }

        private void SendMessageToBackgroundPlayer(ValueSet message)
        {
            BackgroundMediaPlayer.SendMessageToBackground(message);
        }

        public string GetCurrentTrack()
        {
            return (ApplicationSettingsHelper.ReadSettingsValue(ConstantValues.CurrentTrack) as String) ?? String.Empty;
        }

        private void BackgroundTaskInitializationCompleted(IAsyncAction action, AsyncStatus status)
        {
            if (status == AsyncStatus.Completed)
            {
                Debug.WriteLine("Background Audio Task initialized");
            }
            else if (status == AsyncStatus.Error)
            {
                Debug.WriteLine("Background Audio Task could not initialized due to an error ::" + action.ErrorCode.ToString());
            }
        }
    }

}
