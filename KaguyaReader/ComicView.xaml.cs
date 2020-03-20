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
using System.IO;
using System.IO.Compression;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace KaguyaReader
{
    public class MangaImage : INotifyPropertyChanged
    {

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
                //System.Diagnostics.Debug.WriteLine("Set image");
                NotifyPropertyChanged("Image");

            }
            get => _image;
        }

        /*private string _img;
        public string ImagePath
        {
            set
            {
                _img = value;
                //System.Diagnostics.Debug.WriteLine("Set image");
                NotifyPropertyChanged("ImagePath");

            }
            get => _img;
        }*/

        public MangaImage(BitmapImage image)
        {
            Image = image;
            //ImagePath = @"/Assets/test/02.jpg";
        }
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComicView : Page //, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;


        ObservableCollection<MangaImage> ImageCollection = new ObservableCollection<MangaImage>();
        //string fileToOpen;
        //private IEnumerable<MangaImage> _items;

        /*void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        public IEnumerable<MangaImage> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }*/

        private async void loadTestImages()
        {
            //MangaImage img = new MangaImage(await MangaUtils.LoadImageFromAssets(@"Assets\test\01.jpg"));
            //ImageCollection.Add(img);
            List<MangaImage> mangaImages = new List<MangaImage>();
            for (int i = 1; i <= 6; i++)
                ImageCollection.Add(new MangaImage(await MangaUtils.LoadImageFromAssets(@"Assets\test\0" + i.ToString() + ".jpg")));
            
            //Items = mangaImages;
        }
        public ComicView()
        {
            this.InitializeComponent();
            //loadTestImages();
            //MangaCVS.Source = ImageCollection;
            //flipView.ItemsSource = ImageCollection;
        }

        private async void loadFromCBZ(string fileToOpen)
        {
            using (FileStream zipToOpen = new FileStream(fileToOpen, FileMode.Open))
            {

                using (ZipArchive cbz = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    //What absolute GENIUS includes an xml in a cbz?
                    var entries = cbz.Entries.Where(s => (s.Name.EndsWith(".jpg") || s.Name.EndsWith(".png") || s.Name.EndsWith(".jpeg"))).OrderBy(e => e.Name);
                    foreach (var entry in entries)
                    {
                        //TODO: Async it
                        /*
                            using (Stream zipStream = entry.Open())
                            using (FileStream fileStream = new FileStream(...))
                            {
                                await zipStream.CopyToAsync(fileStream);
                            }
                         */
                        using (Stream stream = entry.Open())
                        {
                            var memStream = new MemoryStream();
                            await stream.CopyToAsync(memStream);
                            memStream.Position = 0;
                            var bitmap = new BitmapImage();
                            bitmap.SetSource(memStream.AsRandomAccessStream());
                            //image.Source = bitmap;
                            //Stream fileStream = entry.Open();
                            //BitmapImage image = new BitmapImage();
                            //image.SetSource(stream);
                            ImageCollection.Add(new MangaImage(bitmap));
                        }
                    }
                }
                    

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //string fileToOpen = (String)e.Parameter;
            SimpleMangaData data = (SimpleMangaData)e.Parameter;
            var text = new Windows.UI.Xaml.Documents.Run();
            text.Text = data.Title;
            mainHeading.Inlines.Add(text);


            var text2 = new Windows.UI.Xaml.Documents.Run();
            text2.Text = data.ChapterTitle;
            subHeading.Inlines.Add(text2);

            if (true)
            {
                MangaUtils.ComicTypes fileType = MangaUtils.getTypeFromExtension(Path.GetExtension(data.Path).ToLowerInvariant());
                switch (fileType)
                {
                    case MangaUtils.ComicTypes.ZIP:
                        loadFromCBZ(data.Path);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                loadTestImages();
            }

            MangaCVS.Source = ImageCollection;

        }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //TopBar.Visibility = Visibility.Visible;
        }


        Brush blackBG = new SolidColorBrush(Windows.UI.Colors.Black);
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.Background == blackBG)
                MainGrid.Background = null;
            else
                MainGrid.Background = blackBG;
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }
        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }
    }
}
