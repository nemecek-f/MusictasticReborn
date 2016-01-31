using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public static class MusicModelsFactory
    {
        public static AlbumModel NewAlbumModel(MusicProperties properties, int trackCount)
        {
            return new AlbumModel
            {
                Name = properties.Title,
                Artist = properties.AlbumArtist,
                SongsCount = trackCount,

            };
        }

        public static SongModel NewSongModel(MusicProperties properties, string path, int albumId)
        {
            return new SongModel
            {
                Name = properties.Title,
                Path = path,
                Artist = properties.Artist,
                TrackNumber = (int)properties.TrackNumber,
                Duration = properties.Duration.ToSongDuration(),
                AlbumId = albumId
            };
        }
    }
}
