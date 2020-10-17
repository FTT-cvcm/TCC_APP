using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Receitando.Data;
using Receitando;
using SQLite;
using PCLExt.FileStorage;
using PCLExt.FileStorage.Folders;
using Receitando.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(SQLite_Android))]
namespace Receitando.Droid
{
    class SQLite_Android : ISQLite
    {
        private const string nomeArquivoDB = "AnaliseDB.db3";
        public SQLiteConnection PegarConnection()
        {
            //Cria uma pasta base local para o dispositivo
            var path = new LocalRootFolder();
            //Cria o arquivo
            var arquivo = path.CreateFile(nomeArquivoDB, CreationCollisionOption.OpenIfExists);

            return new SQLite.SQLiteConnection(arquivo.Path);
        }
    }
}