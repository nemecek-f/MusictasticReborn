using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Models
{
    [Table("Playlists")]
    public class PlaylistModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("SongsCount")]
        public int SongsCount { get; set; }

        [Column("PlayTime")]
        public string PlayTime { get; set; }
    }
}
