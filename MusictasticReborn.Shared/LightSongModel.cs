using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusictasticReborn.Shared
{
    public class LightSongModel
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public string ArtistName { get; set; }

        public LightSongModel(string path, string name, string artistName)
        {
            Name = name;
            Path = path;
            ArtistName = artistName;
        }

        private static readonly NullLightSongModel NullModel = new NullLightSongModel();

        public static NullLightSongModel Null => NullModel;
    }

    public class NullLightSongModel : LightSongModel
    {
        internal NullLightSongModel() : base(string.Empty, string.Empty, string.Empty)
        {
            
        }
    }
}
