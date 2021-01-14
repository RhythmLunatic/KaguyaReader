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
        private List<int> loadedImageList;
        private Dictionary<int, BitmapImage> loadedImagePool;
        //private BitmapImage placeholderImage;
        public OnDemandCbzSupplier()
        {
            //placeholderImage = await MangaUtils.LoadImageFromAssets("Square44x44Logo.targetsize-24_altform-unplated.png");
            loadedImagePool = new Dictionary<int, BitmapImage>();
        }
        //private 
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

        private List<int> manageImagePool(int i, BitmapImage bitmap)
        {
            if (loadedImagePool.Count < 5)
            {
                Debug.WriteLine("There are less than 5 elements loaded, so nothing to unload.");
                loadedImagePool.Add(i,bitmap);
                List<int> l = new List<int>();
                foreach (KeyValuePair<int, BitmapImage> kvp in loadedImagePool)
                {
                    l.Add(kvp.Key);
                }
                return l;
            }
            int idxToRemove = -1;
            int furthestFromNewIdx = 0;
            foreach (KeyValuePair<int,BitmapImage> kvp in loadedImagePool)
            {
                /*if (i == num)
                {
                    Debug.WriteLine("An image that was already loaded was requested again ("+i.ToString()+"), so nothing to unload.");
                    return -1;
                }*/
                int distance = Math.Abs(i - kvp.Key);
                if (distance > furthestFromNewIdx)
                {
                    furthestFromNewIdx = distance;
                    idxToRemove = kvp.Key;
                }
            }
            Debug.WriteLine("Plz unload " + idxToRemove + "!");
            //This is safe since it's still being referenced by the manga display
            //Which is probably some form of divine comedy when I still have to keep track of what image is using what bitmap anyways
            loadedImagePool.Remove(idxToRemove);
            loadedImagePool.Add(i,bitmap);
            string numLoaded = "";
            List<int> loadedImageList = new List<int>();
            foreach (KeyValuePair<int, BitmapImage> kvp in loadedImagePool)
            {
                loadedImageList.Add(kvp.Key);
                numLoaded += kvp.Key + ", ";
            }
            Debug.WriteLine(numLoaded);
            return loadedImageList;

            /*if (!loadedImagePool.ContainsKey(i))
            {
                loadedImagePool.Add(i, image);
            }
            else
            {
                throw new Exception("Attempted to add an image to the pool twice.");
            }*/
        }

        public async virtual Task<MangaImage> getImageAtIdx(int i)
        {
            Debug.WriteLine("New image demanded at idx " + i.ToString());
            using (Stream stream = entries.ElementAt(i).Open())
            {
                Debug.WriteLine("Opened file successfully!");
                var memStream = new MemoryStream();
                Debug.WriteLine("Await copytoasync");
                //Crash here because the flipview is not async
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

        public Tuple<BitmapImage, List<int>> getImageAtIdxSync(int i)
        {
            Debug.WriteLine("New image demanded at idx " + i.ToString());
            if (loadedImagePool.ContainsKey(i))
            {
                Debug.WriteLine("Image previously loaded, returning existing BitmapImage");
                return new Tuple<BitmapImage, List<int>>(loadedImagePool[i],null);
            }
            using (Stream stream = entries.ElementAt(i).Open())
            {
                Debug.WriteLine("Opened file successfully!");
                var memStream = new MemoryStream();
                stream.CopyTo(memStream);
                memStream.Position = 0;
                var bitmap = new BitmapImage();
                bitmap.SetSource(memStream.AsRandomAccessStream());
                Debug.WriteLine("Returning decoded image obj...");
                return new Tuple<BitmapImage, List<int>>(bitmap,manageImagePool(i,bitmap));
            }
        }
    }
}
