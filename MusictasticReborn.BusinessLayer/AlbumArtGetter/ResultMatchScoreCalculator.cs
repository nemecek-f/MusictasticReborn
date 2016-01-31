using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.BusinessLayer.AlbumArtGetter
{
    public static class ResultMatchScoreCalculator
    {
        private static readonly Func<AlbumModel, MusicBrainzAlbumsResultRow, int>[] Rules = 
        {
                   (model, result) => String.Equals(model.Name, result.Name, StringComparison.OrdinalIgnoreCase) ? 3 : 0,
                   (model, result) => String.Equals(model.Artist, result.Artist, StringComparison.OrdinalIgnoreCase) ? 2 : 0,
                   (model, result) => result.Score > 90 ? 2 : 0,
                   (model, result) => model.SongsCount == result.NumberOfTracks ? 2 : Math.Abs(model.SongsCount - result.NumberOfTracks) < 2 ? 1 : -1

        };

        public static int CalculateScoreFromRules(AlbumModel model, MusicBrainzAlbumsResultRow result)
        {
            return Rules.Sum(rule => rule(model, result));
        }

        public static int CalculateMatchScore(AlbumModel model, MusicBrainzAlbumsResultRow result)
        {
            int score = 0;

            if (String.Equals(model.Name, result.Name, StringComparison.OrdinalIgnoreCase))
                score += 3;

            if (String.Equals(model.Artist, result.Artist, StringComparison.OrdinalIgnoreCase))
                score += 2;

            if (result.Score > 90)
                score += 2;

            if (model.SongsCount == result.NumberOfTracks)
            {
                score += 2;
            }
            else
            {
                if (Math.Abs(model.SongsCount - result.NumberOfTracks) < 2)
                    score += 1;
                else
                    score -= 1;
            }

            return score;
        }
    }
}
