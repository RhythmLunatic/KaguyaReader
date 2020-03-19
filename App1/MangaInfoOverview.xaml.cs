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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    public struct ChapterListing
    {
        public string Name;
        public string Path;

        public ChapterListing(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MangaInfoOverview : Page
    {
        public ObservableCollection<ChapterListing> chapterList = new ObservableCollection<ChapterListing>();
        private BrowsableManga thisManga;
        public MangaInfoOverview()
        {
            this.InitializeComponent();
        }

        private async void updateCover()
        {
            if (thisManga.Image != null)
                CoverImage.Source = thisManga.Image;
            //There's no point in this...
            /*if (thisManga.thumbIsCached)
            {
                StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalCacheFolder;
                StorageFile file = await localFolder.GetFileAsync(thisManga.imagePath);
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {

                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 500; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    CoverImage.Source = bitmapImage;
                }
            }
            else
            {

                if (!thisManga.hasThumb)
                    return;
                using (IRandomAccessStream fileStream = await FileRandomAccessStream.OpenAsync(thisManga.imagePath, Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 500; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    CoverImage.Source = bitmapImage;
                }
            }*/
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            thisManga = (BrowsableManga)e.Parameter;
            var directory = thisManga.path;
            //string mangaName = Path.GetDirectoryName(directory);
            Header.Text = thisManga.title;
            //Cover.Source = thisManga.imagePath;

            updateCover();
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                chapterList.Add(new ChapterListing(Path.GetFileNameWithoutExtension(file).Replace(thisManga.title + " ", ""), file));
            }
            Listing.ItemsSource = chapterList;
        }

        private void SplitView_LayoutUpdated(object sender, object e)
        {
            //var splitter = sender as SplitView;
            var splitter = splitView;
            if (splitter != null)
            {
                var windowWidth = Window.Current.Bounds.Width / 2;

                splitter.OpenPaneLength = windowWidth <= 500 ? windowWidth : 500;
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }
    }
}
