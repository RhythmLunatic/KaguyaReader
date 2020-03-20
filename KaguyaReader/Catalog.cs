using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace KaguyaReader
{
    public enum CatalogType
    {
        LocalFoler,
        SMB,
        FTP,
        HTTP,
        OneDrive,
        Google_Drive
    }

    public struct Catalog
    {
        public string Name;
        public CatalogType Type;
        public string Path;

        public Catalog(string name, CatalogType type, string path)
        {
            this.Name = name;
            this.Type = type;
            this.Path = path;
        }
    }

    public sealed class CatalogManager
    {

        public ObservableCollection<Catalog> catalogs = new ObservableCollection<Catalog>();

        public static CatalogManager Instance { get; } = new CatalogManager();

        //This is something you can't actually do unless you request broadFilesystemAccess in appxmanifest
        //So if you put your own folder there and now it's crashing... now you know why
        //This probably shouldn't be here in the first place
        private void addTestCatalogs()
        {
            for (int i = 0; i < 20; i++)
                catalogs.Add(new Catalog("Test Catalog", CatalogType.LocalFoler, @"\\192.168.1.109\USBDriveSudo\Comics and Manga"));
        }

        public void addNewCatalog(Catalog newCatalog)
        {
            catalogs.Add(newCatalog);
        }

        public string serializeCatalogs()
        {
            //return JsonConvert.SerializeObject(new Catalog("Test Catalog", CatalogType.LocalFoler, @"\\192.168.1.109\USBDriveSudo\Comics and Manga"));
            return JsonConvert.SerializeObject(catalogs);
        }

        CatalogManager()
        {
            addTestCatalogs();
        }

    }
}
