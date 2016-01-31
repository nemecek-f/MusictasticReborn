using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusictasticReborn.BusinessLayer.Extensions
{
    public class MediaLibraryChangeEventArgs : EventArgs
    {
        public string Message { get; set; }

        public int AlbumCount { get; set; }

        public int SongCount { get; set; }

        public int TotalSongCount { get; set; }

        public MediaLibraryChangeEventArgs(string message)
        {
            Message = message;
        }
    }
}
