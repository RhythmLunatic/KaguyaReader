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
using Windows.Storage;
using Windows.Storage.Pickers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KaguyaReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        //private CatalogManager CATALOGMAN;
        public MainPage()
        {
            this.InitializeComponent();
            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            Current = this;
            //SampleTitle.Text = FEATURE_NAME;

            // Caching your main page is good practice, this makes it snappy for the user to return to "home" of your app.
            this.NavigationCacheMode = NavigationCacheMode.Required;
            adjustSize();

            //TopAppBar.
        }
        
        private void adjustSize()
        {
            //System.Diagnostics.Debug.WriteLine(bounds.Width);
            //For some reason if the app starts with a width under 1008, the splitter is visible
            if (Window.Current.Bounds.Width < 1008)
            {
                Splitter.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else 
            {
                SplitViewDisplayMode prevDisplayMode = Splitter.DisplayMode;
                Splitter.DisplayMode = SplitViewDisplayMode.Inline;
                if (prevDisplayMode == SplitViewDisplayMode.Overlay)
                    Splitter.IsPaneOpen = true;
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            Splitter.IsPaneOpen = !Splitter.IsPaneOpen;
        }

        private void Splitter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            adjustSize();
        }

        private void button_Click_1(object sender, TappedRoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Navigate to the next page, with info in the parameters whether to enable the title bar UI or not.
            rootFrame.Navigate(typeof(ComicView));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TopBarButtonHolder.PrimaryCommands.Clear();
            //System.Diagnostics.Debug.WriteLine((sender as ListBox).SelectedIndex);
            //I don't know how how to make it better
            switch ((sender as ListBox).SelectedIndex)
            {
                case 0:
                    MainFrame.Navigate(typeof(LibraryView), Splitter);
                    break;
                case 3:
                    MainFrame.Navigate(typeof(CatalogView), Splitter);
                    break;
                case 5:
                    MainFrame.Navigate(typeof(SettingsView));
                    break;
            }
        }

        private void ListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {

            //MainFrame.Navigate(typeof(LibraryView), TopBarButtonHolder);
        }

        private void ListBoxItem_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            //MainFrame.Navigate(typeof(SettingsView));
        }

        private async void ListBoxItem_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            //OutputTextBlock.Text = "";

            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".cbz");
            openPicker.FileTypeFilter.Add(".cbr");
            openPicker.FileTypeFilter.Add(".cb7");
            openPicker.FileTypeFilter.Add(".zip");
            openPicker.FileTypeFilter.Add(".rar");
            openPicker.FileTypeFilter.Add(".7z");
            StorageFile file = await openPicker.PickSingleFileAsync();
        }

        private void MainMenu_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (Splitter.DisplayMode == SplitViewDisplayMode.Overlay)
                Splitter.IsPaneOpen = false; //Close it on selection
        }
    }
}
