using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Interfaces;

namespace MusictasticReborn.BusinessLayer.Models
{
    public class PlayableArtist : IPlayable
    {
        private List<SongModel> _songs;

        public PlayableArtist(IEnumerable<SongModel> artistSongs)
        {
            _songs = new List<SongModel>(artistSongs);
        }

        public IEnumerable<SongModel> GetSongs()
        {
            return _songs;
        }
    }
}
