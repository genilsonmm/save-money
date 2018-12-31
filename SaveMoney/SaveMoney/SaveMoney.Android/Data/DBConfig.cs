using SaveMoney.Data;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;
using System;
using Xamarin.Forms;

[assembly: Dependency(typeof(DBConfig))]
namespace SaveMoney.Data
{
    public class DBConfig : IDBConfig
    {
        private string _diretory;
        private ISQLitePlatform _platform;

        public string Diretory
        {
            get
            {
                if (string.IsNullOrEmpty(_diretory))
                {
                    _diretory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                }
                return _diretory;
            }
        }

        public ISQLitePlatform Platform
        {
            get
            {
                if (_platform == null)
                {
                    _platform = new SQLitePlatformAndroid();
                }
                return _platform;
            }
        }
    }
}