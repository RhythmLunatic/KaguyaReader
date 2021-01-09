using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.IO.Compression;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;

namespace KaguyaReader
{
    //This should be abstract but idk how to define an abstract async task
    public class FiletypeSuppliers
    {
        public int numEntries { get; protected set; }
        /*public virtual void cacheImage()
        {

        }*/
        public async Task<MangaImage> getImageAtIdx(int i)
        {
            return new MangaImage(await MangaUtils.LoadImageFromAssets(@"Assets\test\cover.png"));
        }
    }

    //I don't know who on earth keeps their comics as folders but
    public class FolderSupplier : FiletypeSuppliers
    {

    }

    /// <summary>
    /// Basically it's supposed to be a polymorphic class that has functions for loading the next page on demand
    /// </summary>
    public class OnDemandCbzSupplier : FiletypeSuppliers
    {
        private IRandomAccessStream fileOpenHandle;
        private ZipArchive cbz;
        private List<ZipArchiveEntry> entries;
        public OnDemandCbzSupplier()
        {

        }
        public async Task<bool> openFile(StorageFile fileToOpen)
        {
            fileOpenHandle = await fileToOpen.OpenAsync(FileAccessMode.Read);
            Debug.WriteLine("Opened zip");

            cbz = new ZipArchive(fileOpenHandle.AsStream(), ZipArchiveMode.Read);
            //Get only valid entries... sometimes metadata is included
            entries = cbz.Entries.Where(s => MangaUtils.ValidImageTypes.Contains(Path.GetExtension(s.Name))).OrderBy(e => e.Name).ToList();
            numEntries = entries.Count();
            Debug.WriteLine("Got " + numEntries.ToString() + " entries");
            return true;
        }

        public async virtual Task<MangaImage> getImageAtIdx(int i)
        {
            Debug.WriteLine("New image demanded at idx " + i.ToString());
            using (Stream stream = entries.ElementAt(i).Open())
            {
                Debug.WriteLine("Opened file successfully!");
                var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);
                memStream.Position = 0;
                var bitmap = new BitmapImage();
                bitmap.SetSource(memStream.AsRandomAccessStream());
                //image.Source = bitmap;
                //Stream fileStream = entry.Open();
                //BitmapImage image = new BitmapImage();
                //image.SetSource(stream);
                Debug.WriteLine("Returning decoded image obj...");
                return new MangaImage(bitmap);
            }
        }

    }
}
