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

namespace KaguyaReader
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

        //It doesn't need to be async when the image is already loaded, right?
        private void updateCover()
        {
            if (thisManga.Image != null)
                CoverImage.Source = thisManga.Image;
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
            foreach (var file in Directory.EnumerateFiles(directory).Where(s => MangaUtils.ValidComicFileTypes.Contains(Path.GetExtension(s).ToLowerInvariant())))
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

        private void Listing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            Frame rootFrame = Window.Current.Content as Frame;
            var m = chapterList[Listing.SelectedIndex];
            //var n = ;
            rootFrame.Navigate(typeof(ComicView), new SimpleMangaData(m.Path, thisManga.title, m.Name));
        }
    }
}
