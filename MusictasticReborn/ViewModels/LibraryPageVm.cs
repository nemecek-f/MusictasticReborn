using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using MusictasticReborn.BusinessLayer;
using MusictasticReborn.BusinessLayer.AlbumArtGetter;
using MusictasticReborn.BusinessLayer.Helpers;
using MusictasticReborn.BusinessLayer.Models;
using MusictasticReborn.UserControls.Extensions;
using SQLite;

namespace MusictasticReborn.ViewModels
{
    public class LibraryPageVm : BindableVm
    {
        public ObservableCollection<AlbumModel> Albums => _albums;

        public ObservableCollection<SongModel> Songs => _songs;

        public ObservableCollection<ArtistModel> Artists
        {
            get
            {
                if (_artists == null)
                {
                    LoadArtists().Wait();
                }

                return _artists;
            }
        }

        private ObservableCollection<AlbumModel> _albums;

        private ObservableCollection<SongModel> _songs;

        private ObservableCollection<ArtistModel> _artists;

        private SQLiteAsyncConnection _db;

        private PlaylistManager _playlistManager;

        public LibraryPageVm()
        {
            _db = DatabaseHelper.OpenConnection();

            _playlistManager = new PlaylistManager();
        }

        public IList<IJumpListItem> PrepareItemsForSongsJumpList()
        {
            List<IJumpListItem> jumpListItems = new List<IJumpListItem>(Songs.Count + (Songs.Count / 20));

            int listLength = Songs.Count;

            string firstAlbumName = Albums.First(album => album.Id == Songs[0].AlbumId).Name;
            int previousAlbumId = Songs[0].AlbumId;

            SolidColorBrush headerBrush = (SolidColorBrush)App.Current.Resources["ThemeBrush"];

            jumpListItems.AddRange(Songs);
            jumpListItems.Insert(0, new StandardJumpListHeader(firstAlbumName, headerBrush));
            listLength++;

            int injectedHeadersCount = 0;

            for (int i = 1; i < listLength; i++)
            {
                if (i - injectedHeadersCount >= Songs.Count)
                    break;

                int currentAlbumId = Songs[i - injectedHeadersCount].AlbumId;

                if (previousAlbumId != currentAlbumId)
                {   // New album, inject new header
                    
                    previousAlbumId = currentAlbumId;

                    jumpListItems.Insert(i + 1,
                                    new StandardJumpListHeader(Albums.First(a => a.Id == previousAlbumId).Name, headerBrush));

                    injectedHeadersCount++;

                    listLength++;
                }
            }

            return jumpListItems;
        }

        public void StartPlayingArtist(ArtistModel artist)
        {
            var artistsSongs =
                    Songs.Where(song => song.Artist.Equals(artist.Name, StringComparison.OrdinalIgnoreCase));

            MusicPlayerWrapper.Instance.Play(new PlayableArtist(artistsSongs));
        }

        public async Task LoadAlbums()
        {
            if (_albums != null)
                return;

            var albumList = await _db.Table<AlbumModel>().ToListAsync();
            _albums = new ObservableCollection<AlbumModel>(albumList);

            await Task.Run(() => MusicPlayerWrapper.Instance.BuildArtCache(_albums));

            OnPropertyChanged("Albums");
        }

        public async Task LoadSongs()
        {
            if (_songs != null)
                return;

            var songList = await _db.Table<SongModel>().ToListAsync();
            _songs = new ObservableCollection<SongModel>(songList);

            OnPropertyChanged("Songs");
        }

        public async Task LoadArtists()
        {
            if (_albums == null)
                await LoadAlbums();

            var artists = _albums.Select(a => new ArtistModel() { Name = a.Artist }).Distinct();

            _artists = new ObservableCollection<ArtistModel>(artists);

            OnPropertyChanged("Artists");
        }

        public async Task AddAlbumArt()
        {
            AlbumArtGetter albumArtGetter = new AlbumArtGetter();

            await albumArtGetter.FindAndSetAlbumArtAsync(Albums);

            //var tempModel = new AlbumModel()
            //{
            //    Artist = "Ed Guy",
            //    Name = "Age of the Joker",
            //    SongsCount = 11,
            //    ArtPath = "ms-appdata:///Local/f7000dc8-afb3-4307-804d-1ed7a370c116.png"
            //};

            //var slipknotModel = new AlbumModel
            //{
            //    Artist = "Slipknot",
            //    Name = "All Hope Is Gone",
            //    SongsCount = 15
            //};

            //Albums.Add(slipknotModel);
            

            //if (!Albums.Any())
            //{
            //    await LoadAlbums();
            //}

            //foreach (var album in Albums)
            //{
            //    await albumArtGetter.FindAndSetAlbumArtAsync(album);
            //}
            
        }
    }
}
