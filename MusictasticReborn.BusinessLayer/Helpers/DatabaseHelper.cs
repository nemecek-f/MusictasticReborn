using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Models;
using SQLite;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public static class DatabaseHelper
    {
        public static SQLiteAsyncConnection OpenConnection()
        {
            return new SQLiteAsyncConnection("musictastic.db");
        }

        public static async Task PrepareDb()
        {
            var connection = OpenConnection();

            await connection.CreateTableAsync<AlbumModel>();
            await connection.CreateTableAsync<SongModel>();
            await connection.CreateTableAsync<PlaylistModel>();
            await connection.CreateTableAsync<PlaylistMapping>();

        }

        public static async Task UpdateDb()
        {
            await CreateTableIfNotExists<PlaylistModel>("Playlists");

            await CreateTableIfNotExists<PlaylistMapping>("PlaylistMapping");
        }

        private static async Task CreateTableIfNotExists<T>(string tableName) where T : new()
        {
            var connection = OpenConnection();

            var count = await connection.ExecuteScalarAsync<int>(String.Format("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}'", tableName));

            if (count == 0)
                await connection.CreateTableAsync<T>();
        }
    }
}
