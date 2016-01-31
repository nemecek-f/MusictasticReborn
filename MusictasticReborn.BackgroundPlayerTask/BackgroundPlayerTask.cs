using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Playback;
using MusictasticReborn.Shared;

namespace MusictasticReborn.BackgroundPlayerTask
{
    enum ForegroundAppStatus
    {
        Active,
        Suspended,
        Unknown
    }

    public sealed class BackgroundPlayerTask : IBackgroundTask
    {
        private SystemMediaTransportControls _systemMediaTransportControl;
        private BackgroundTaskDeferral _deferral;
        private AutoResetEvent _backgroundTaskStarted = new AutoResetEvent(false);
        private bool _backgroundtaskrunning = false;
        private ForegroundAppStatus _foregroundAppState = ForegroundAppStatus.Unknown;

        private QueueManager.Queue _queue;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            InitializeAndSetMediaTransportControl();

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);
            taskInstance.Task.Completed += Taskcompleted;

            var value = ApplicationSettingsHelper.ReadResetSettingsValue(ConstantValues.AppState);

            if (value == null)
                _foregroundAppState = ForegroundAppStatus.Unknown;
            else
                _foregroundAppState = (ForegroundAppStatus)Enum.Parse(typeof(ForegroundAppStatus), value.ToString());

            BackgroundMediaPlayer.Current.CurrentStateChanged += Current_CurrentStateChanged;

            UpdateQueue();

            _queue.TrackChanged += playList_TrackChanged;

            BackgroundMediaPlayer.MessageReceivedFromForeground += BackgroundMediaPlayer_MessageReceivedFromForeground;

            if (_foregroundAppState != ForegroundAppStatus.Suspended)
            {
                ValueSet message = new ValueSet { { ConstantValues.BackgroundTaskStarted, "" } };
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }

            _backgroundTaskStarted.Set();
            _backgroundtaskrunning = true;
            
            ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.BackgroundTaskState, ConstantValues.BackgroundTaskRunning);
            _deferral = taskInstance.GetDeferral();
        }

        private void playList_TrackChanged(QueueManager.Queue sender, object args)
        {
            UpdateSystemMediaTransportControl(sender.CurrentTrack);
            ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.CurrentTrack, sender.CurrentTrack.Name);

            if (_foregroundAppState == ForegroundAppStatus.Active)
            {
                ValueSet message = new ValueSet { { ConstantValues.Trackchanged, sender.CurrentTrack.Name } };
                BackgroundMediaPlayer.SendMessageToForeground(message);
            }
        }

        private void InitializeAndSetMediaTransportControl()
        {
            _systemMediaTransportControl = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaTransportControl.ButtonPressed += systemMediaTransportControl_ButtonPressed;
            _systemMediaTransportControl.PropertyChanged += systemMediaTransportControl_PropertyChanged;
            _systemMediaTransportControl.IsEnabled = true;
            _systemMediaTransportControl.IsPauseEnabled = true;
            _systemMediaTransportControl.IsPlayEnabled = true;
            _systemMediaTransportControl.IsNextEnabled = true;
            _systemMediaTransportControl.IsPreviousEnabled = true;
        }

        private void BackgroundMediaPlayer_MessageReceivedFromForeground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            foreach (string key in e.Data.Keys)
            {
                switch (key.ToLower())
                {
                    case ConstantValues.AppSuspended:
                        _foregroundAppState = ForegroundAppStatus.Suspended;
                        ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.CurrentTrack, _queue.CurrentTrack.Name);
                        break;
                    case ConstantValues.AppResumed:
                        _foregroundAppState = ForegroundAppStatus.Active;
                        break;
                    case ConstantValues.StartPlayback:
                        Debug.WriteLine("Start playback message received from foreground");
                        UpdateQueue();
                        StartPlayback();
                        break;
                    case ConstantValues.SkipNext:
                        SkipToNext();
                        break;
                    case ConstantValues.SkipPrevious:
                        SkipToPrevious();
                        break;
                    case ConstantValues.RefreshPlaylist:
                        UpdateQueue(true);
                        BackgroundMediaPlayer.Current.Pause();
                        _queue.StartPlaying();
                        BackgroundMediaPlayer.Current.Play();
                        break;
                }
            }
        }

        private void UpdateQueue(bool overrideActive = false)
        {
            string toDesearilize =
                    (string)ApplicationSettingsHelper.ReadSettingsValue(ConstantValues.CurrentPlaylist);

            Debug.WriteLine("Deserialized string: " + toDesearilize);

            var songs =
                SongsSerializer.DeserializeIntoModels(toDesearilize);

            if (_queue != null)
            {
                _queue.UpdateSongs(songs);
            }
            else
            {
                _queue = QueueManager.CreateNewPlaylist(songs);
            }
        }

        private void StartPlayback()
        {
            try
            {
                if (_queue.CurrentTrack == LightSongModel.Null)
                {
                    var currentTrackName = ApplicationSettingsHelper.ReadSettingsValue(ConstantValues.CurrentTrack);
                    var currentTrackPosition = ApplicationSettingsHelper.ReadResetSettingsValue(ConstantValues.Position);
                    if (currentTrackName != null)
                    {

                        if (currentTrackPosition == null)
                        {

                            _queue.StartTrackAt((string)currentTrackName);
                        }
                        else
                        {

                            _queue.StartTrackAt((string)currentTrackName, TimeSpan.Parse((string)currentTrackPosition));
                        }
                    }
                    else
                    {

                        _queue.StartPlaying();
                    }
                }
                else
                {
                    BackgroundMediaPlayer.Current.Play();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void SkipToNext()
        {
            _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Changing;
            _queue.SkipToNext();
        }

        private void SkipToPrevious()
        {
            _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Changing;
            _queue.SkipToPrevious();
        }

        private void Current_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.CurrentState == MediaPlayerState.Playing)
            {
                _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Playing;
            }
            else if (sender.CurrentState == MediaPlayerState.Paused)
            {
                _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Paused;
            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            try
            {
                ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.CurrentTrack, _queue.CurrentTrack.Name);
                ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.Position, BackgroundMediaPlayer.Current.Position.ToString());
                ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.BackgroundTaskState, ConstantValues.BackgroundTaskCancelled);
                ApplicationSettingsHelper.SaveSettingsValue(ConstantValues.AppState, Enum.GetName(typeof(ForegroundAppStatus), _foregroundAppState));
                _backgroundtaskrunning = false;

                _queue.TrackChanged -= playList_TrackChanged;

                _systemMediaTransportControl.ButtonPressed -= systemMediaTransportControl_ButtonPressed;
                _systemMediaTransportControl.PropertyChanged -= systemMediaTransportControl_PropertyChanged;

                BackgroundMediaPlayer.Shutdown();
            }
            catch (Exception ex)
            {

            }

            _deferral.Complete();
        }

        private void Taskcompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            _deferral.Complete();
        }

        private void UpdateSystemMediaTransportControl(LightSongModel track)
        {
            _systemMediaTransportControl.PlaybackStatus = MediaPlaybackStatus.Playing;
            _systemMediaTransportControl.DisplayUpdater.Type = MediaPlaybackType.Music;
            _systemMediaTransportControl.DisplayUpdater.MusicProperties.Title = track.Name;
            _systemMediaTransportControl.DisplayUpdater.MusicProperties.Artist = "";
            _systemMediaTransportControl.DisplayUpdater.Update();
        }

        void systemMediaTransportControl_PropertyChanged(SystemMediaTransportControls sender, SystemMediaTransportControlsPropertyChangedEventArgs args)
        {

        }

        void systemMediaTransportControl_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    if (!_backgroundtaskrunning)
                    {
                        bool result = _backgroundTaskStarted.WaitOne(2000);
                        if (!result)
                            throw new Exception("Background Task didnt initialize in time");
                    }
                    else
                    {
                        BackgroundMediaPlayer.Current.Play();
                    }

                    break;
                case SystemMediaTransportControlsButton.Pause:

                    try
                    {
                        BackgroundMediaPlayer.Current.Pause();
                    }
                    catch (Exception ex)
                    {

                    }
                    break;
                case SystemMediaTransportControlsButton.Next:
                    _queue.SkipToNext();
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    _queue.SkipToPrevious();
                    break;
            }
        }
    }
}
