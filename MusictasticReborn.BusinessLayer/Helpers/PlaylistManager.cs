using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Models;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public class PlaylistManager
    {
        private readonly SQLiteAsyncConnection _db;

        public PlaylistManager()
        {
            _db = DatabaseHelper.OpenConnection();
        }

        public async Task<IEnumerable<PlaylistModel>> GetPlaylists()
        {
            return await _db.Table<PlaylistModel>().ToListAsync();
        }

        public async Task<IEnumerable<SongModel>> GetSongsFromPlaylist(PlaylistModel playlist)
        {
            var mappings =
                await _db.Table<PlaylistMapping>().Where(mapping => mapping.PlayListId == playlist.Id).ToListAsync();

            HashSet<int> songIds = new HashSet<int>(mappings.Select(m => m.SongId));

            return await _db.Table<SongModel>().Where(song => songIds.Contains(song.Id)).ToListAsync();
        }

        public async Task CreateNewPlaylistAsync(string name, IEnumerable<SongModel> songs)
        {
            PlaylistModel newPlaylist = new PlaylistModel() { Name = name };

            var songList = songs.ToList();

            double totalLength = songList.Sum(song => ParseSongDuration(song.Duration).TotalSeconds);

            newPlaylist.SongsCount = songList.Count;

            newPlaylist.PlayTime = TimeSpan.FromSeconds(totalLength).ToPlaylistDuration();

            await _db.InsertAsync(newPlaylist);

            await AddSongsToPlaylist(newPlaylist, songList);

        }

        public async Task AddSongsToPlaylist(PlaylistModel target, IEnumerable<SongModel> songs)
        {
            var songsList = songs.ToList();

            foreach (var song in songsList)
            {
                await _db.InsertAsync(new PlaylistMapping(target.Id, song.Id));
            }

            target.SongsCount += songsList.Count;

            double newDuration = ParsePlaylistDuration(target.PlayTime).TotalSeconds +
                                 (songsList.Sum(song => ParseSongDuration(song.Duration).TotalSeconds));

            target.PlayTime = TimeSpan.FromSeconds(newDuration).ToPlaylistDuration();
        }

        public async Task DeletePlaylist(PlaylistModel target)
        {
            var toDeleteMappings =
                await _db.Table<PlaylistMapping>().Where(mapping => mapping.PlayListId == target.Id).ToListAsync();

            foreach (var mapping in toDeleteMappings)
            {
                await _db.DeleteAsync(mapping);
            }

            await _db.DeleteAsync(target);
        }

        private TimeSpan ParseSongDuration(string duration)
        {
            return TimeSpan.ParseExact(duration, "m':'ss", null);
        }

        private TimeSpan ParsePlaylistDuration(string duration)
        {
            string preFormatted = duration.Replace("h ", ":").Replace("m", string.Empty);

            return TimeSpan.ParseExact(preFormatted, "h':'mm", null);
        }
    }
}
