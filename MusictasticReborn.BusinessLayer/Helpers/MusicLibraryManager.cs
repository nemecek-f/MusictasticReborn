using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Extensions;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public class MusicLibraryManager
    {
        private readonly MusicDataGetter _musicGetter;

        public event EventHandler<MediaLibraryChangeEventArgs> FinishedAlbumsLookup;

        public event EventHandler<MediaLibraryChangeEventArgs> NewAlbumsAdded;

        public event EventHandler<MediaLibraryChangeEventArgs> NewSongsScanned;

        public MusicLibraryManager()
        {
            _musicGetter = new MusicDataGetter();

            _musicGetter.NewSongsDiscovered += _musicGetter_NewSongsDiscovered;
        }

        void _musicGetter_NewSongsDiscovered(object sender, MediaLibraryChangeEventArgs e)
        {
            OnNewSongsScanned(e);
        }

        private void OnNewSongsScanned(MediaLibraryChangeEventArgs e)
        {
            NewSongsScanned?.Invoke(this, e);
        }

        private void OnFinishedAlbumsLookup(int albumsCount)
        {
            FinishedAlbumsLookup?.Invoke(this, new MediaLibraryChangeEventArgs("") {AlbumCount = albumsCount});
        }

        private void OnNewAlbumsAdded(int count)
        {
            NewAlbumsAdded?.Invoke(this, new MediaLibraryChangeEventArgs("") { AlbumCount = count});
        }


        public async Task RescanMusicAlbums()
        {
            var dbConnection = DatabaseHelper.OpenConnection();
            var albumsInDb = await dbConnection.Table<AlbumModel>().ToListAsync();

            var foundAlbums = await _musicGetter.GetMusicByAlbumsAsync();

            int newAlbums = 0;

            // Check for new albums
            foreach (var album in foundAlbums.Where(album => !albumsInDb.Contains(album)))
            {
                await dbConnection.InsertAsync(album);
                await dbConnection.InsertAllAsync(album.Songs);
                newAlbums++;
            }

            if (newAlbums > 0)
                OnNewAlbumsAdded(newAlbums);

            // Delete DB albums no longer present in the file system 
            foreach (var album in albumsInDb.Where(album => !foundAlbums.Contains(album)))
            {
                await dbConnection.DeleteAsync(album);

                int albumId = album.Id;
                foreach (var song in await dbConnection.Table<SongModel>()
                            .Where(s => s.AlbumId == albumId).ToListAsync())
                {
                    await dbConnection.DeleteAsync(song);
                }
            }
        }
    }
}
