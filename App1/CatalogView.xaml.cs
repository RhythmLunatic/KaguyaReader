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
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CatalogView : Page
    {
        SplitView splitter;

        public CatalogView()
        {
            this.InitializeComponent();
            //System.Diagnostics.Debug.WriteLine(CatalogManager.Instance.serializeCatalogs());
            CataloguesCVS.Source = CatalogManager.Instance.catalogs;
        }



        private async void PickALocalFolder()
        {
            // Clear previous returned folder name, if it exists, between iterations of this scenario
            //OutputTextBlock.Text = "";

            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add(".cbz");
            folderPicker.FileTypeFilter.Add(".cbr");
            folderPicker.FileTypeFilter.Add(".cb7");
            folderPicker.FileTypeFilter.Add(".zip");
            folderPicker.FileTypeFilter.Add(".rar");
            folderPicker.FileTypeFilter.Add(".7z");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                //OutputTextBlock.Text = "Picked folder: " + folder.Name;
                CatalogManager.Instance.addNewCatalog(new Catalog(folder.Name, CatalogType.LocalFoler, folder.Name));

            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            splitter = (SplitView)e.Parameter;
            //CommandBar topBar = (CommandBar)e.Parameter;
            /*topBar.PrimaryCommands.Add(new AppBarButton
            {
                Template=(ControlTemplate)this.Resources["AddFolderButton"]
            });*/
            //topBar.PrimaryCommands.Add((AppBarButton)this.Resources["AddFolderButton"]);
            /*topBar.PrimaryCommands.Add(new AppBarButton
            {
                Icon = new SymbolIcon(Symbol.Add),
                Flyout = new MenuFlyout
                {
                    Items =
                    {
                        (new MenuFlyoutItem{ Text="Add local folder"}).Click += PickALocalFolder(object sender, RoutedEventArgs e),
                        new MenuFlyoutItem{ Text="Add SMB shared folder" },
                        new MenuFlyoutItem{ Text="Add FTP server folder" },
                        new MenuFlyoutItem{ Text="Add new HTTP directory index folder" },
                        new MenuFlyoutItem{ Text="Add new OneDrive folder" },
                        new MenuFlyoutItem{ Text="Add new Google Drive folder" },
                    }
                }
            });*/
            /*AppBarButton editButton = new AppBarButton();
            editButton.Icon = new SymbolIcon(Symbol.Add);
            //editButton.
            topBar.PrimaryCommands.Add(editButton);*/
            //topBar.PrimaryCommands.Add((MenuFlyout)this.Resources["DropDown"]);
            //topBar.PrimaryCommands.Add()
            //System.Diagnostics.Debug.WriteLine("Navigated to!");
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Tapped!");
            splitter.IsPaneOpen = !splitter.IsPaneOpen;
        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            PickALocalFolder();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ListView_Tapped(object sender, TappedRoutedEventArgs e)
        {

            var index = (sender as ListView).SelectedIndex;
            //Frame rootFrame = Window.Current.Content as Frame;
            Frame rootFrame = this.Frame;
            rootFrame.Navigate(typeof(CatalogBrowser), CatalogManager.Instance.catalogs[index]);
        }
    }
}
