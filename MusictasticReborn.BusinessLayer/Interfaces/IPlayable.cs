using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Models;

namespace MusictasticReborn.BusinessLayer.Interfaces
{
    public interface IPlayable
    {
        IEnumerable<SongModel> GetSongs();
    }
}
