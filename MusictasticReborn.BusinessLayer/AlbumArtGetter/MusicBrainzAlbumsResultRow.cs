using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusictasticReborn.BusinessLayer.AlbumArtGetter
{
    public class MusicBrainzAlbumsResultRow
    {
        public int Score { get; set; }

        public string Name { get; set; }

        public string Artist { get; set; }

        public int NumberOfTracks { get; set; }

        public int MatchScore { get; set; }

        public string Url { get; set; }

        public override string ToString()
        {
            return String.Format("Album {0}, by artist: {1}. Number of tracks: {2}", Name, Artist, NumberOfTracks);
        }
    }
}
