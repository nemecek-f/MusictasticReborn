using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Models
{
    [Table("Genres")]
    public class MusicGenreModel
    {
        [Column("Name")]
        public string Name { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
