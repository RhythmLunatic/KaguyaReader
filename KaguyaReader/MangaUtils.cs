using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Storage;

namespace KaguyaReader
{
    public static class MangaUtils
    {
        public enum ComicTypes {
            ZIP,
            RAR,
            SEVENZIP,
            INVALID
        }

        public static List<String> ValidComicFileTypes = new List<string> { ".cbz", ".cbr", ".cb7", ".zip", ".rar", ".7z" };
        public static List<String> ValidImageTypes = new List<string> { ".jpg", ".jpeg", ".png", ".bmp" };

        //inb4 some dude gets mad because I checked the extension instead of the actual file signature
        //I probably should check the file sig instead of the extension though
        public static ComicTypes getTypeFromExtension(string extension)
        {
            switch (extension)
            {
                case ".cbz":
                case ".zip":
                    return ComicTypes.ZIP;
                case ".rar":
                case ".cbr":
                    return ComicTypes.RAR;
                case ".cb7":
                case ".7z":
                    return ComicTypes.SEVENZIP;
                //Shouldn't reach this point
                default:
                    return ComicTypes.INVALID;
            }
        }

        public static async Task<BitmapImage> LoadImageAsync(string imagePath)
        {
            using (IRandomAccessStream fileStream = await FileRandomAccessStream.OpenAsync(imagePath, Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap 
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelWidth = 500; //match the target Image.Width, not shown
                await bitmapImage.SetSourceAsync(fileStream);
                System.Diagnostics.Debug.WriteLine(imagePath + " finished loading thumb!");
                return bitmapImage;
                //Image = bitmapImage;
                //System.Diagnostics.Debug.WriteLine(title + " finished loading thumb!");
            }
        }

        //TODO: Silence exceptions being printed in the debug log
        public static async Task<bool> DoesCachedFileExistAsync(string fileName)
        {

            //System.Diagnostics.Debug.WriteLine("Does " + fileName + " exist?");
            try
            {
                await ApplicationData.Current.LocalCacheFolder.GetFileAsync(fileName);
                //System.Diagnostics.Debug.WriteLine("True!");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<BitmapImage> LoadImageFromCache(string fileName)
        {
            StorageFile file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(fileName);
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {

                // Set the image source to the selected bitmap 
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.DecodePixelWidth = 500; //match the target Image.Width, not shown
                await bitmapImage.SetSourceAsync(fileStream);
                //Image = bitmapImage;
                //Debug.WriteLine("Got cached thumb: " + fileToTest);
                return bitmapImage;
            }
        }

        //As in the assets folder
        public static async Task<BitmapImage> LoadImageFromAssets(string fileName)
        {
            StorageFile file;
            try
            {
                file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\" + fileName);
            }
            catch
            {
                throw new Exception("The file "+ fileName+" doesn't exist.");
                //return new BitmapImage();
            }
            
            
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap 
                BitmapImage bitmapImage = new BitmapImage();
                //bitmapImage.DecodePixelWidth = 500; //match the target Image.Width, not shown
                await bitmapImage.SetSourceAsync(fileStream);
                //Image = bitmapImage;
                //Debug.WriteLine("Got cached thumb: " + fileToTest);
                return bitmapImage;
            }
        }

        public static async Task<StorageFile> getFileFromAssets(string fileName)
        {
            StorageFile file;
            try
            {
                file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\" + fileName);
            }
            catch
            {
                throw new Exception("The file " + fileName + " doesn't exist.");
                //return new BitmapImage();
            }
            return file;
        }
    }

    /// <summary>
    /// Holds the information needed for the comic viewer and nothing else.
    /// This should probably be renamed.
    /// </summary>
    public struct SimpleMangaData
    {
        public StorageFile File;
        public string Title;
        public string ChapterTitle;
        //public int Chapter;
        public SimpleMangaData(StorageFile path, string title, string chapter)
        {
            File = path;
            Title = title;
            ChapterTitle = chapter;
            //Chapter = chapter;
        }
    }


}
