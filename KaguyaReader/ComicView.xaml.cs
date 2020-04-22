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

        // What is the program's last known full-screen state?
        // We use this to detect when the system forced us out of full-screen mode.
        private bool isLastKnownFullScreen = ApplicationView.GetForCurrentView().IsFullScreenMode;

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

        /*private async void loadTestImages()
        {
            //MangaImage img = new MangaImage(await MangaUtils.LoadImageFromAssets(@"Assets\test\01.jpg"));
            //ImageCollection.Add(img);
            List<MangaImage> mangaImages = new List<MangaImage>();
            for (int i = 1; i <= 6; i++)
                ImageCollection.Add(new MangaImage(await MangaUtils.LoadImageFromAssets(@"Assets\test\0" + i.ToString() + ".jpg")));
            
            //Items = mangaImages;
        }*/
        public ComicView()
        {
            this.InitializeComponent();
            //Unsubscribe from the toggled event, set toggle, then resubscribe
            FullscreenToggle.Toggled -= FullscreenToggle_Toggled;
            FullscreenToggle.IsOn = isLastKnownFullScreen;
            FullscreenToggle.Toggled += FullscreenToggle_Toggled;
            //loadTestImages();
            //MangaCVS.Source = ImageCollection;
            //flipView.ItemsSource = ImageCollection;
        }

        private async Task<int> loadFromCBZ(StorageFile fileToOpen)
        {
            using (IRandomAccessStream fileStream = await fileToOpen.OpenAsync(FileAccessMode.Read))
            {

                using (ZipArchive cbz = new ZipArchive(fileStream.AsStream(), ZipArchiveMode.Read))
                {
                    //What absolute GENIUS includes an xml in a cbz?
                    var entries = cbz.Entries.Where(s => MangaUtils.ValidImageTypes.Contains(Path.GetExtension(s.Name))).OrderBy(e => e.Name);
                    //This is obviously a bad idea since the manga reader will eat all the memory and crash
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
            //TODO: DON'T DO THIS, THIS IS A TERRIBLE IDEA
            //Slider.Maximum = ImageCollection.Count()-1;
            return ImageCollection.Count();
        }

        //I couldn't figure out how to assign 0 by default
        private async Task<int> unimplementedTask()
        {
            return 0;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
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

            MangaUtils.ComicTypes fileType = MangaUtils.getTypeFromExtension(data.File.FileType.ToLowerInvariant());
            Task<int> count;
            switch (fileType)
            {
                case MangaUtils.ComicTypes.ZIP:
                    count = loadFromCBZ(data.File);
                    break;
                default:
                    count = unimplementedTask();
                    break;
            }

            MangaCVS.Source = ImageCollection;
            Slider.Maximum = await count-1;
            //Slider.Maximum = ImageCollection.Count();
            System.Diagnostics.Debug.WriteLine(ImageCollection.Count() + " pages in comic.");

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

        private void NavigationView_BackRequested(object sender, TappedRoutedEventArgs args)
        {
            On_BackRequested();
        }

        private void FullscreenToggle_Toggled(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                //rootPage.NotifyUser("Exiting full screen mode", NotifyType.StatusMessage);
                isLastKnownFullScreen = false;
                // The SizeChanged event will be raised when the exit from full screen mode is complete.
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    //rootPage.NotifyUser("Entering full screen mode", NotifyType.StatusMessage);
                    isLastKnownFullScreen = true;
                    // The SizeChanged event will be raised when the entry to full screen mode is complete.
                }
                else
                {
                    //rootPage.NotifyUser("Failed to enter full screen mode", NotifyType.ErrorMessage);
                }
            }
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
