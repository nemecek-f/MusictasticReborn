using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MusictasticReborn.Shared
{
    public static class ApplicationSettingsHelper
    {
        public static object ReadResetSettingsValue(string key)
        {
            if (!HasKey(key))
            {
                
                return null;
            }
            else
            {
                var value = ApplicationData.Current.LocalSettings.Values[key];
                ApplicationData.Current.LocalSettings.Values.Remove(key);
                return value;
            }
        }

        public static object ReadSettingsValue(string key)
        {
            if (!HasKey(key))
            {

                return null;
            }
            else
            {
                var value = ApplicationData.Current.LocalSettings.Values[key];
                return value;
            }
        }

        public static bool HasKey(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
        }
      
        public static void SaveSettingsValue(string key, object value)
        {
            if (!HasKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

    }
}
