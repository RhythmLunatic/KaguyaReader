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
    public sealed partial class BlankPage1 : Page //, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<MangaImage> ImageCollection = new ObservableCollection<MangaImage>();

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
        public BlankPage1()
        {
            this.InitializeComponent();
            loadTestImages();
            MangaCVS.Source = ImageCollection;
            //flipView.ItemsSource = ImageCollection;
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
    }
}
