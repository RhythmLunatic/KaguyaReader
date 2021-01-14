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

    //arrrgggggghhhhhhahghghghhghghghghgh UWP is so bad
    /*public class ImageMemoryManager
    {

        //For archives manual memory management is needed...
        private BitmapImage placeholderImage;
        ObservableCollection<MangaImage> ImageCollection;
        private Dictionary<int, BitmapImage> imageMemoryManager;
        public BitmapImage getImageAtIdx(int idx)
        {

        }
    }*/

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
        OnDemandCbzSupplier supplier = new OnDemandCbzSupplier();
        BitmapImage placeholderImage;

        //Actual position in the cbz
        int trueIndex = 0;
        //Position in the flipView
        int _Index = default(int);
        public int Index { 
            get { return _Index; } 
            set
            {
                //If < 0, the flipView has unloaded! Do not do anything, garbage collector will clean it
                if (value < 0)
                {
                    return;
                }
                List<int> imagesLoaded = null;
                if (value > 1)
                {
                    var result = supplier.getImageAtIdxSync(value - 1);
                    ImageCollection[value - 1].Image = result.Item1;
                    imagesLoaded = result.Item2;
                }
                if (value + 1 < ImageCollection.Count)
                {
                    var result = supplier.getImageAtIdxSync(value + 1);
                    ImageCollection[value + 1].Image = result.Item1;
                    imagesLoaded = result.Item2;
                }
                if (imagesLoaded != null)
                {
                    //ImageCollection[imageToUnload].Image = placeholderImage;
                    //Wrongest way to free memory
                    for (int i = 0; i < ImageCollection.Count; i++)
                    {
                        if (!imagesLoaded.Contains(i) && ImageCollection[i].Image != placeholderImage)
                            ImageCollection[i].Image = placeholderImage;
                    }
                }
                /*if (value > _Index)
                    trueIndex++;
                else if (value < _Index)
                    trueIndex--;
                Debug.WriteLine("flipView pos "+value.ToString()+" | comic pos: "+trueIndex.ToString());*/

                    //If < 0, the flipView has unloaded! Do not do anything, garbage collector will clean it
                    /*if (value < 0)
                    {
                        return;
                    }
                    else if (value > _Index)
                    {
                        if (value + 2 <= supplier.numEntries-1)
                        {

                            Debug.WriteLine("Unloading image at front now...");
                            //Because I can't remove from the pool, just do this...
                            //It replaces the oldest image with the newest one so the oldest one gets removed from the memory pool
                            if (value > 2)
                                ImageCollection[value - 2] = ImageCollection[value];


                            //TODO: Actually collection size should just be the same size as the number of pages in the manga
                            if (value+2>ImageCollection.Count-1)
                            {

                                Debug.WriteLine("Loading new image at idx " + (value + 2).ToString() + " and appending to back");
                                ImageCollection.Add(supplier.getImageAtIdxSync(value + 2));
                                Debug.WriteLine("Collection size is now " + ImageCollection.Count.ToString());
                            }
                            else
                            {
                                Debug.WriteLine("Loading new image at idx " + (value + 2).ToString());
                                ImageCollection[value + 2] = supplier.getImageAtIdxSync(value + 2);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Already hit end? " + supplier.numEntries.ToString());
                        }
                    }
                    else if (value < _Index)
                    {
                        if (value - 2 >= 0)
                        {
                            Debug.WriteLine("Loading new image at idx " + (value - 2).ToString());
                            ImageCollection[value-2] = supplier.getImageAtIdxSync(value - 2);
                            Debug.WriteLine("Unloading image at end...");
                            if (value + 2 < ImageCollection.Count - 1)
                                ImageCollection[value + 2] = ImageCollection[value];
                        }
                    }*/
                _Index = value;
            }
        }

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

            //Set up the share dialog
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            //Unsubscribe from the toggled event, set toggle, then resubscribe
            FullscreenToggle.Toggled -= FullscreenToggle_Toggled;
            FullscreenToggle.IsOn = isLastKnownFullScreen;
            FullscreenToggle.Toggled += FullscreenToggle_Toggled;
            //loadTestImages();
            //MangaCVS.Source = ImageCollection;
            //flipView.ItemsSource = ImageCollection;
        }

        //TODO: Oh boy I love side effects
        //This function should be removed and the supplier opening more explicit
        private async Task<int> loadFromCBZ(StorageFile fileToOpen)
        {
            if (await supplier.openFile(fileToOpen))
            {
                return supplier.numEntries;
            }
            throw new System.IO.IOException("Failed to open CBZ.");
            return 0;
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
            Slider.Maximum = await count - 1;
            //Slider.Maximum = ImageCollection.Count();
            Debug.WriteLine(count + " pages in comic.");

            Debug.WriteLine("Loading first 5 pages on demand...");
            for (int i = 0; i < Math.Min(count.Result, 5); i++)
            {
                ImageCollection.Add(await supplier.getImageAtIdx(i));
            }

            placeholderImage = await MangaUtils.LoadImageFromAssets("Square44x44Logo.targetsize-24_altform-unplated.png");
            //These are all separate images because we want to replace the bitmap in any image without affecting another
            while (ImageCollection.Count < count.Result)
            {
                ImageCollection.Add(new MangaImage(placeholderImage));
            }

        }

        private void image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (TopBar.Visibility == Visibility.Visible)
            {
                TopBar.Visibility = Visibility.Collapsed;
                BottomBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                TopBar.Visibility = Visibility.Visible;
                BottomBar.Visibility = Visibility.Visible;
            }
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
                //Need to clear the loaded images from memory manually. Don't know why, it should be cleared when you assign a new image pool.
                ImageCollection.Clear();
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

        private void flipView_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share Text Example";
            request.Data.Properties.Description = "An example of how to share text.";
            request.Data.SetText("Hello World!");
        }

        private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("A");

        }
    }
}
