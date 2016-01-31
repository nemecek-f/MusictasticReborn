using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MusictasticReborn.BusinessLayer.Interfaces;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Models
{
    [Table("Albums")]
    public class AlbumModel : IPlayable, INotifyPropertyChanged
    {
        private string _artPath;

        protected bool Equals(AlbumModel other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Artist, other.Artist) && SongsCount == other.SongsCount;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Artist != null ? Artist.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ SongsCount;
                return hashCode;
            }
        }

        public static bool operator ==(AlbumModel left, AlbumModel right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AlbumModel left, AlbumModel right)
        {
            return !Equals(left, right);
        }

        public AlbumModel()
        {
            Songs = new List<SongModel>(15);
        }

        [Column("Name")]
        public string Name { get; set; }

        [Column("GenreId")]
        public int GenreId { get; set; }

        [Column("Artist")]
        public string Artist { get; set; }

        [PrimaryKey]
        public int Id { get; set; }

        [Column("ArtPath")]
        public string ArtPath
        {
            get { return _artPath; }
            set
            {
                _artPath = value;
                OnPropertyChanged();
            }
        }

        [Ignore]
        public BitmapImage Art { get; set; }

        [Column("SongsCount")]
        public int SongsCount { get; set; }

        [Ignore]
        public List<SongModel> Songs { get; set; }
        
        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AlbumModel) obj);
        }

        public IEnumerable<SongModel> GetSongs()
        {
            return Songs;
        }

        public IEnumerable<string> GetSongPaths()
        {
            return Songs.Select(s => s.Path);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
