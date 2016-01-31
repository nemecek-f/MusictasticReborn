using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MusictasticReborn.BusinessLayer.Helpers
{
    public static class StorageHelper
    {
        private static readonly StorageFolder Local = ApplicationData.Current.LocalFolder;

        public static async Task<string> SaveImageAsync(Stream image)
        {
            string fileName = Guid.NewGuid() + ".png";

            StorageFile storageFile = await Local.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (Stream outputStream = await storageFile.OpenStreamForWriteAsync())
            {
                await image.CopyToAsync(outputStream);
            }
            
            return "ms-appdata:///Local/" + fileName;
        }
    }
}
