﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusictasticReborn.Shared
{
    public class ConstantValues
    {
        public const string CurrentTrack = "trackname";
        public const string CurrentPlaylist = "currPlaylist";
        public const string BackgroundTaskStarted = "BackgroundTaskStarted";
        public const string BackgroundTaskRunning = "BackgroundTaskRunning";
        public const string BackgroundTaskCancelled = "BackgroundTaskCancelled";
        public const string AppSuspended = "appsuspend";
        public const string AppResumed = "appresumed";
        public const string StartPlayback = "startplayback";
        public const string SkipNext = "skipnext";
        public const string Position = "position";
        public const string AppState = "appstate";
        public const string BackgroundTaskState = "backgroundtaskstate";
        public const string SkipPrevious = "skipprevious";
        public const string Trackchanged = "songchanged";
        public const string ForegroundAppActive = "Active";
        public const string ForegroundAppSuspended = "Suspended";
        public const string RefreshPlaylist = "playlistrefresh";

        public class FileExtensions
        {
            public static readonly string[] Music = new string[]
            {
                ".mp3", 
            };

            public static readonly string[] Images = new string[]
            {
                ".png", ".jpg", ".jpeg" 
            };
        }
    }
}
