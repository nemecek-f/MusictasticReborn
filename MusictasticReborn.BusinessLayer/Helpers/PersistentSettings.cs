using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusictasticReborn.Shared;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public static class PersistentSettings
    {
        public static bool AlwaysNavigateToPlayScreen
        {
            get { return (bool)ApplicationSettingsHelper.ReadSettingsValue("navigateToPlay"); }
            set { ApplicationSettingsHelper.SaveSettingsValue("navigateToPlay", value); }
        }
    }
}
