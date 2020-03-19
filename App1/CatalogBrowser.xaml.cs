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
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.IO.Compression;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
//using Ionic.Zip;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    public enum BROWSABLE_TYPE {
        FOLDER_WITH_MANGA,
        SINGLE_FILE,
        FOLDER_NO_MANGA,
        //FOLDER_WITH_IMAGES, //This probably shouldn't be supported
        //GO_UP_A_DIRECTORY //They can press the back button
    }


    public class BrowsableManga : INotifyPropertyChanged
    {
        public String title;
        public string path;
        public BROWSABLE_TYPE type;
        private string _imagePath;
        public bool hasThumb; //This should probably mean "has local thumb" or something idk
        public bool thumbIsCached;
        private string _nfo;

        //Remove this
        private string imagePath
        {
            get
            {
                if (hasThumb)
                    return _imagePath;
                else
                    return @"Assets/cover.png";
            }
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        //void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private BitmapImage _image;
        public BitmapImage Image
        {
            set
            {
                _image = value;
                NotifyPropertyChanged("Image");

            }
            get => _image;
        }


        private async void loadImage()
        {
            if (hasThumb)
            {
                Image = await MangaUtils.LoadImageAsync(imagePath);
            }
            else if (type == BROWSABLE_TYPE.SINGLE_FILE || type == BROWSABLE_TYPE.FOLDER_WITH_MANGA)
            {
                //StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                //Surely there is a less bad way to do this right
                foreach(string fileToTest in new string[]{ safeThumbName() + ".png", safeThumbName() + ".jpg" })
                {
                    if (await MangaUtils.DoesCachedFileExistAsync(fileToTest))
                    {
                        Image = await MangaUtils.LoadImageFromCache(fileToTest);
                        return;
                    }
                }
                GetImageFromCBZ();

            }
        }

        /*public async Task<BitmapImage> GetImageAsync()
        {

        }*/
        bool inLibrary = false;

        public BrowsableManga(string title, string path, BROWSABLE_TYPE type, string imagePath)
        {
            this.title = title;
            this.path = path;
            this.type = type;
            this._imagePath = imagePath;
            hasThumb = true;
            loadImage();
        }

        public BrowsableManga(string title, string path, BROWSABLE_TYPE type)
        {
            this.title = title;
            this.path = path;
            this.type = type;
            loadImage();
            //GetImageFromCBZ();
        }

        private string safeThumbName()
        {
            return Path.GetInvalidFileNameChars().Aggregate(this.title, (current, c) => current.Replace(c, '-')) + "_Cover";
        }
        
        /// <summary>
        /// Gets the first image in the cbz.
        /// It's public because you can refresh the cover from the right click menu. THough I dunno why you'd use it
        /// </summary>
        /// TODO: Async it pls. Walking directories is slow.
        public async void GetImageFromCBZ()
        {
            //string fileName = safeThumbName();
            string fileToOpen;
            if (type == BROWSABLE_TYPE.SINGLE_FILE)
            {
                fileToOpen = path;
            }
            //If they don't want to add a cover.jpg/png then just find the latest manga I guess
            else if (type == BROWSABLE_TYPE.FOLDER_WITH_MANGA)
            {
                var files = Directory.EnumerateFiles(path).Where(s => MangaUtils.ValidComicFileTypes.Contains(Path.GetExtension(s).ToLowerInvariant()));
                files.OrderBy(e => e);
                fileToOpen = files.FirstOrDefault();
            }
            else
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine(fileToOpen);
            MangaUtils.ComicTypes fileType = MangaUtils.getTypeFromExtension(Path.GetExtension(fileToOpen).ToLowerInvariant());
            switch (fileType)
            {
                case MangaUtils.ComicTypes.ZIP:
                    using (FileStream zipToOpen = new FileStream(fileToOpen, FileMode.Open))
                    {

                        using (ZipArchive cbz = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                        {
                            //What absolute GENIUS includes an xml in a cbz?
                            var entries = cbz.Entries.Where(s => (s.Name.EndsWith(".jpg") || s.Name.EndsWith(".png") || s.Name.EndsWith(".jpeg"))).OrderBy(e => e.Name);
                            var firstEntry = entries.FirstOrDefault();
                            System.Diagnostics.Debug.WriteLine(firstEntry);
                            Stream fileStream = firstEntry.Open();

                            string extension = Path.GetExtension(firstEntry.Name).ToLowerInvariant();
                            if (extension == ".jpeg") //Fuck jpeg
                                extension = ".jpg";
                            string fileName = safeThumbName() + extension;
                            //fileStream.Read()
                            //what the fuck
                            using (MemoryStream ms = new MemoryStream())
                            {
                                fileStream.CopyTo(ms);
                                StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                                StorageFile newFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                //hasThumb = true;
                                thumbIsCached = true;
                                _imagePath = fileName;
                                //Debugging only
#if DEBUG
                                using (BinaryWriter outputFile = new BinaryWriter(File.OpenWrite(Path.Combine(Environment.GetEnvironmentVariable("temp"), fileName))))
                                {
                                    System.Diagnostics.Debug.WriteLine("Writing " + Path.Combine(Environment.GetEnvironmentVariable("temp"), fileName));
                                    outputFile.Write(ms.ToArray());
                                }
#endif

                                await FileIO.WriteBytesAsync(newFile, ms.ToArray());
                            }
                            //FileIO.Write
                            //await FileIO.Wri
                            /*foreach (var entry in entries)
                                System.Diagnostics.Debug.Write(entry);*/
                            /*
                             * "But wait don't we already have the image in memory why are you loading it from disk again"
                             * You fix it, it's async and I don't care
                             */
                            Image = await MangaUtils.LoadImageFromCache(fileName);
                        }
                    }
                    break;
            }
            //StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //StorageFile newFile = await localFolder.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);
        }

    }

    public static class FolderHandler
    {


        /*private static BrowsableManga generateRandomManga()
        {
            return new Manga("Test Manga", "Test Author", 1, 0, false, "None");
        }*/

        //TODO: This needs to be changed to an async method. Scan each directory async so the window doesn't freeze.
        public static ObservableCollection<BrowsableManga> GetMangas(string path)
        {
            ObservableCollection<BrowsableManga> mangas = new ObservableCollection<BrowsableManga>();
            
            //Filter only valid files
            var files = Directory.EnumerateFiles(path).Where(s => MangaUtils.ValidComicFileTypes.Contains(Path.GetExtension(s).ToLowerInvariant()));
            foreach (string file in files)
            {
                mangas.Add(new BrowsableManga(Path.GetFileNameWithoutExtension(file), file, BROWSABLE_TYPE.SINGLE_FILE));
            }
            var directories = Directory.EnumerateDirectories(path);
            foreach(string directory in directories)
            {
                var filesInDir = Directory.EnumerateFiles(directory).Where(s => MangaUtils.ValidComicFileTypes.Contains(Path.GetExtension(s).ToLowerInvariant()));
                //System.Diagnostics.Debug.WriteLine(filesInDir.ToList().Count);
                if (filesInDir.Any())
                {
                    //TODO: Check if cover.jpg/png/bmp exists
                    if (File.Exists(Path.Combine(directory,"cover.png")))
                    {
                        mangas.Add(new BrowsableManga(Path.GetFileNameWithoutExtension(directory), directory, BROWSABLE_TYPE.FOLDER_WITH_MANGA, Path.Combine(directory, "cover.png")));
                    }
                    else if (File.Exists(Path.Combine(directory, "cover.jpg")))
                        mangas.Add(new BrowsableManga(Path.GetFileNameWithoutExtension(directory), directory, BROWSABLE_TYPE.FOLDER_WITH_MANGA, Path.Combine(directory, "cover.jpg")));
                    else
                        mangas.Add(new BrowsableManga(Path.GetFileNameWithoutExtension(directory), directory, BROWSABLE_TYPE.FOLDER_WITH_MANGA));
                }
                else
                {
                    mangas.Add(new BrowsableManga(Path.GetFileNameWithoutExtension(directory), directory, BROWSABLE_TYPE.FOLDER_NO_MANGA));
                }
            }
            return mangas;

        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CatalogBrowser : Page
    {
        Catalog currentCatalog;
        ObservableCollection<BrowsableManga> mangas;
        MenuFlyout sharedFlyout;
        public CatalogBrowser()
        {
            this.InitializeComponent();
            sharedFlyout = (MenuFlyout)Resources["SampleContextMenu"];
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            currentCatalog = (Catalog)e.Parameter;
            Header.Text = currentCatalog.Name;
            if (currentCatalog.Type == CatalogType.LocalFoler)
            {
                mangas = FolderHandler.GetMangas(currentCatalog.Path);
                MangaCVS.Source = mangas;
            }
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void MangaListing_Tapped(object sender, TappedRoutedEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine(mangas[MangaListing.SelectedIndex].title);
            var selectedManga = mangas[MangaListing.SelectedIndex];
            if (selectedManga.type == BROWSABLE_TYPE.FOLDER_WITH_MANGA)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MangaInfoOverview), selectedManga);
            }
        }



        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            var img = sender as Image;
            if (img != null)
            {
                //GetImageFromCBZ(img);
            }
        }

        // Handles system-level BackRequested events and page-level back button Click events
        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private BrowsableManga GetDataModelForCurrentListViewFlyout()
        {
            // Obtain the ListViewItem for which the user requested a context menu.
            var listViewItem = sharedFlyout.Target;

            // Get the data model for the ListViewItem.
            return (BrowsableManga)MangaListing.ItemFromContainer(listViewItem);
        }
        private void MangaListing_ContextRequested(UIElement sender, ContextRequestedEventArgs e)
        {
            // Walk up the tree to find the ListViewItem.
            // There may not be one if the click wasn't on an item.
            var requestedElement = (FrameworkElement)e.OriginalSource;
            while ((requestedElement != sender) && !(requestedElement is GridViewItem))
            {
                requestedElement = (FrameworkElement)VisualTreeHelper.GetParent(requestedElement);
            }
            if (requestedElement != sender)
            {
                // The context menu request was indeed for a ListViewItem.
                var model = (BrowsableManga)MangaListing.ItemFromContainer(requestedElement);
                //MainPage rootPage = MainPage.Current;
                Point point;

                if (e.TryGetPosition(requestedElement, out point))
                {
                    Debug.WriteLine($"Showing flyout for {model.title} at {point}");
                    //rootPage.NotifyUser($"Showing flyout for {model.title} at {point}", NotifyType.StatusMessage);
                    sharedFlyout.ShowAt(requestedElement, point);
                }
                else
                {
                    // Not invoked via pointer, so let XAML choose a default location.
                    //rootPage.NotifyUser($"Showing flyout for {model.title} at default location", NotifyType.StatusMessage);
                    sharedFlyout.ShowAt(requestedElement);
                }

                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            BrowsableManga model = GetDataModelForCurrentListViewFlyout();
            model.GetImageFromCBZ();
            //Debug.WriteLine(model.title + " clicked");
            //rootPage.NotifyUser($"Item: {model.Title}, Action: Open", NotifyType.StatusMessage);
        }
    }
}
