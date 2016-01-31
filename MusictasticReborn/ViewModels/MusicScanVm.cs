using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.BusinessLayer.Helpers;

namespace MusictasticReborn.ViewModels
{
    public class MusicScanVm : BindableVm
    {
        private MusicDataGetter _getter;
        private int _songFound;

        public MusicScanVm()
        {
            _getter = new MusicDataGetter();

            _getter.NewSongsDiscovered += _getter_NewSongsDiscovered;
        }

        void _getter_NewSongsDiscovered(object sender, BusinessLayer.Extensions.MediaLibraryChangeEventArgs e)
        {
            SongFound = e.TotalSongCount;
        }

        public int SongFound
        {
            get { return _songFound; }
            set
            {
                _songFound = value;
                OnPropertyChanged();
            }
        }
    }
}
