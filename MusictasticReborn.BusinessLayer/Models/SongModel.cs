using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using MusictasticReborn.BusinessLayer.Helpers;
using MusictasticReborn.BusinessLayer.Interfaces;
using MusictasticReborn.UserControls.Extensions;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Models
{
    [Table("Songs")]
    public class SongModel : IPlayable, IJumpListItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("AlbumId")]
        public int AlbumId { get; set; }

        [Column("Path")]
        public string Path { get; set; }

        [Column("Artist")]
        public string Artist { get; set; }

        [Column("Duration")]
        public string Duration { get; set; }

        [Column("TrackNumber")]
        public int TrackNumber { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<SongModel> GetSongs()
        {
            yield return this;
        }

        public IEnumerable<string> GetSongPaths()
        {
            yield return Path;
        }

        public SongModel()
        {
            
        }

        [Ignore]
        public string Text => Name;

        [Ignore]
        public bool IsHeader => false;

        [Ignore]
        public SolidColorBrush Background { get; private set; }
    }
}
