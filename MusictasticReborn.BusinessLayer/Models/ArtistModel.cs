using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Models
{
    [Table("Artists")]
    public class ArtistModel
    {
        protected bool Equals(ArtistModel other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(ArtistModel left, ArtistModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ArtistModel left, ArtistModel right)
        {
            return !Equals(left, right);
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("GenreId")]
        public int GenreId { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ArtistModel) obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
