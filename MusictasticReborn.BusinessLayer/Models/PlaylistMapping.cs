using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Models
{
    [Table("PlaylistMapping")]
    public class PlaylistMapping
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("PlaylistId")]
        public int PlayListId { get; set; }

        [Column("SongId")]
        public int SongId { get; set; }

        public PlaylistMapping(int playlistId, int songId)
        {
            SongId = songId;
            PlayListId = playlistId;
        }

        public PlaylistMapping()
        {
            
        }
    }
}
