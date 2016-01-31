using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.Annotations;
using MusictasticReborn.BusinessLayer.Helpers;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.ViewModels
{
    public class TempVm : INotifyPropertyChanged
    {
        private ObservableCollection<SongModel> _songs = new ObservableCollection<SongModel>();

        private readonly MusicDataGetter _musicGetter = new MusicDataGetter();

        public ObservableCollection<SongModel> Songs
        {
            get { return _songs; }
            set
            {
                _songs = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadSongs()
        {
            try
            {
                var songs = await DatabaseHelper.OpenConnection().Table<SongModel>().ToListAsync();
                _songs = new ObservableCollection<SongModel>(songs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public ObservableCollection<AlbumModel> Albums { get { return _albums; } }

        private ObservableCollection<AlbumModel> _albums = new ObservableCollection<AlbumModel>();

        public async Task LoadAlbums()
        {
            _albums = new ObservableCollection<AlbumModel>(await _musicGetter.GetMusicByAlbumsAsync());
        }

        public bool AlbumsLoaded
        {
            get { return _albums.Count > 0; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
