using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class DataFileManager
    {
        private static DataFileManager _Internal;
        public static DataFileManager Use(string dbName, string dataPath)
        {
            if (_Internal == null)
                _Internal = new DataFileManager(dbName, dataPath);

            return _Internal;
        }
        public static DataFileManager Use()
        {
            if (_Internal == null)
                throw new ApplicationException("Must initialize with dbName and dataPath first");

            return _Internal;
        }

        public const string GRAPHDB_CONFIGNAME = "{0}.dbconfig";
        public const string DATAFILE_FILENAME = "{0}-{1}-{2}.data";
        public const string TRANSACTIONFILE_FILENAME = "{0}.xaction";

        public GraphDBData DBData { get; set; }
        public string TransactionFile
        {
            get
            {
                return Path.Combine(DBData.DataFilePath, string.Format(TRANSACTIONFILE_FILENAME, DBData.DatabaseName));
            }
        }


        private DataFileManager(string dbName, string dataPath)
        {
            var configpath = Path.Combine(dataPath, string.Format(GRAPHDB_CONFIGNAME, dbName));
            if (File.Exists(configpath))
            {
                var configData = System.IO.File.ReadAllText(configpath);
                DBData = Newtonsoft.Json.JsonConvert.DeserializeObject<GraphDBData>(configData);
            }
            else
            {
                DBData = new GraphDBData
                {
                    DatabaseName = dbName,
                    DataFilePath = dataPath,
                    MaxSize = GraphDBEngine.MAX_ITEMS
                };
                var configData = Newtonsoft.Json.JsonConvert.SerializeObject(DBData);
                File.WriteAllText(configpath, configData);
            }
        }
        //todo: validate GraphDB config file



    }
}
