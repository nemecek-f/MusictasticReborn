using System;
using System.Linq;
using System.Collections.Generic;

namespace MusictasticReborn.Shared
{
    public static class SongsSerializer
    {

        public static string Serialize(IEnumerable<LightSongModel> songs)
        {
            var ligthSongModels = songs as IList<LightSongModel> ?? songs.ToList();
            
            List<string> toJoin = new List<string>(ligthSongModels.Count * 3);

            foreach (var model in ligthSongModels)
            {
                toJoin.Add(model.Name);
                toJoin.Add(model.ArtistName);
                toJoin.Add(model.Path);
            }

            return String.Join(";", toJoin);
        }

        public static IEnumerable<LightSongModel> DeserializeIntoModels(string input)
        {
            string[] parts = input.Split(';');

            List<LightSongModel> songs = new List<LightSongModel>();
            
            for (int i = 0; i < parts.Length - 1; i = i + 3)
            {
                songs.Add(new LightSongModel(parts[i + 2], parts[i], parts[i + 1]));
            }

            return songs;
        }

        private static string GetBaseFolderPath(string fullPath)
        {
            return fullPath.Substring(0, fullPath.LastIndexOf("\\", System.StringComparison.Ordinal));
        }
    }
}
