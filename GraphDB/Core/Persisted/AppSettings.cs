using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class AppSettings
    {
        private static AppSettings _Internal;
        public static AppSettings Use()
        {
            if (_Internal == null)
                _Internal = new AppSettings();

            return _Internal;
        }
        public static void Set(string dbName, string dataPath)
        {
            var configpath = Path.Combine(dataPath, string.Format(GRAPHDB_CONFIGNAME, dbName));
            if (File.Exists(configpath))
            {
                var configData = System.IO.File.ReadAllText(configpath);
                _Internal = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(configData);
            }
            else
            {
                _Internal = new AppSettings
                {
                    DatabaseName = dbName,
                    DataFilePath = dataPath,
                    MaxSize = GraphDBEngine.MAX_ITEMS
                };
                var configData = Newtonsoft.Json.JsonConvert.SerializeObject(_Internal);
                File.WriteAllText(configpath, configData);
            }
            //todo: validate GraphDB config file
        }

        public const int DATABLOCKSIZE = 10;
        public const string GRAPHDB_CONFIGNAME = "{0}.dbconfig";
        public const string DATAFILE_FILENAME = "{0}-{1}-{2}.data";
        public const string TRANSACTIONFILE_FILENAME = "{0}.xaction";

        public string DatabaseName { get; set; }
        public string DataFilePath { get; set; }
        public int MaxSize { get; set; }

        private AppSettings()
        {

        }

    }
}
