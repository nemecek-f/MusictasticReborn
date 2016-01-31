using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.BusinessLayer.Cache
{
    public class AlbumArtCache
    {
        private Dictionary<int, string> _albumArtPaths = new Dictionary<int, string>();

        public void AddAlbumArt(int albumId, string path)
        {
            _albumArtPaths[albumId] = path;
        }

        public string GetAlbumArt(int albumId)
        {
            return _albumArtPaths[albumId];
        }

        public void BuildCache(IEnumerable<AlbumModel> albums)
        {
            foreach (var album in albums)
            {
                _albumArtPaths[album.Id] = album.ArtPath;
            }
        }

        public bool IsEmpty
        {
            get { return _albumArtPaths.Count == 0; }
        }
    }
}
