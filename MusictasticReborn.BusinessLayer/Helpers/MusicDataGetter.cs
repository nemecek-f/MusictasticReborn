using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;
using MusictasticReborn.BusinessLayer.Extensions;
using MusictasticReborn.BusinessLayer.Models;
using MusictasticReborn.Shared;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public class MusicDataGetter
    {
        private int _albumCount = 0;

        private int _songCount = 0;

        public event EventHandler<MediaLibraryChangeEventArgs> NewSongsDiscovered;

        private void OnNewSongsDiscovered(int count)
        {
            NewSongsDiscovered?.Invoke(this, new MediaLibraryChangeEventArgs("") { SongCount = count, TotalSongCount = _songCount });
        }

        public async Task<List<AlbumModel>> GetMusicByAlbumsAsync()
        {
            List<AlbumModel> albums = new List<AlbumModel>();

            var musicFolders = await GetMusicFoldersAsync();

#if DEBUG
            musicFolders = musicFolders.Take(2);
#endif

            foreach (var musicFolder in musicFolders)
            {
                _albumCount++;

                var musicFiles = await musicFolder.GetFilesAsync();

                var folderProps = await musicFolder.Properties.GetMusicPropertiesAsync();

                try
                {
                    StorageItemThumbnail albumThumbnail = await musicFolder.GetScaledImageAsThumbnailAsync(ThumbnailMode.MusicView);

                    var tempBitmap = new BitmapImage();

                    await tempBitmap.SetSourceAsync(albumThumbnail.CloneStream());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                var album = MusicModelsFactory.NewAlbumModel(folderProps, musicFiles.Count);

                _songCount += musicFiles.Count;

                OnNewSongsDiscovered(musicFiles.Count);

                album.Id = _albumCount;

                foreach (var file in musicFiles)
                {
                    var properties = await file.Properties.GetMusicPropertiesAsync();

                    album.Songs.Add(MusicModelsFactory.NewSongModel(properties, file.Path, _albumCount));
                }

                if (String.IsNullOrEmpty(album.Artist))
                {
                    album.Artist = album.Songs[0].Artist;
                }

                if (!album.Songs.Any())
                    continue;

                await SetAlbumArtIfExists(album);

                albums.Add(album);
            }

            return albums;
        }

        private async Task SetAlbumArtIfExists(AlbumModel album)
        {
            var albumFolder = GetFolderPathFromFileName(album.Songs[0].Path);

            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(albumFolder);

            if (folder != null)
            {
                var files = await folder.GetFilesAsync();

                var imageFile = files.FirstOrDefault(file => ConstantValues.FileExtensions.Images.Contains(file.FileType));

                if (imageFile != null)
                {
                    // oh my, finally we have image art
                    album.ArtPath = imageFile.Path;
                }
            }
        }

        private string GetFolderPathFromFileName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('\\'));
        }

        public async Task<List<SongModel>> GetSomeSongsAsync()
        {
            List<SongModel> songs = new List<SongModel>();

            var music = await GetMusicFoldersAsync();

            foreach (StorageFolder folder in music.Skip(1).Take(1))
            {
                var musicFiles = await folder.GetFilesAsync();

                _songCount += musicFiles.Count;

                OnNewSongsDiscovered(musicFiles.Count);

                var folderProps = await folder.Properties.GetMusicPropertiesAsync();

                foreach (var file in musicFiles.Take(5))
                {
                    var properties = await file.Properties.GetMusicPropertiesAsync();

                    songs.Add(new SongModel
                    {
                        Name = properties.Title,
                        Path = file.Path,
                        Artist = properties.Artist,
                        Duration = properties.Duration.ToSongDuration(),
                    });
                }
            }

            return songs;
        }

        public async Task<IEnumerable<StorageFolder>> GetMusicFoldersAsync()
        {
            return await KnownFolders.MusicLibrary.GetFoldersAsync(CommonFolderQuery.GroupByAlbum);
        }
    }
}
