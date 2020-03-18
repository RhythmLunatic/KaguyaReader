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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{
    public class Manga
    {
        public String title;
        public String artist;
        public int lastReadChapter;
        public int lastUpdated;
        public bool isOngoing;
        public String source;
        public string imagePath = @"???";

        public Manga(string title, string artist, int lastReadChapter, int lastUpdated, bool isOngoing, string source)
        {
            this.title = title;
            this.artist = artist;
            this.lastReadChapter = lastReadChapter;
            this.lastUpdated = lastUpdated;
            this.isOngoing = isOngoing;
            this.source = source;
        }

    }
    public class MangaHandler
    {


        private static Manga generateRandomManga()
        {
            return new Manga("Test Manga", "Test Author", 1, 0, false, "None");
        }

        public static ObservableCollection<Manga> GetMangas()
        {
            ObservableCollection<Manga> mangas = new ObservableCollection<Manga>();
            for (int i = 0; i < 10; i++)
                mangas.Add(generateRandomManga());
            return mangas;

        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryView : Page
    {
        SplitView splitter;
        public LibraryView()
        {
            this.InitializeComponent();
            MangaCVS.Source = MangaHandler.GetMangas();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            splitter = (SplitView)e.Parameter;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            splitter.IsPaneOpen = !splitter.IsPaneOpen;
        }
    }
}
