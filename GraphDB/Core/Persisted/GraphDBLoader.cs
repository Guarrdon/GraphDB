using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class GraphDBLoader:IDisposable
    {
        public IndexFileManager IndexManager { get; set; }
        public DataFileManager DataManager { get; set; }
        public TransactionFileManager TransactionManager { get; set; }


        public GraphDBEngine DB { get; set; }

        public GraphDBLoader()
        {
            //todo: improve this...don't hardcode
            AppSettings.Set("GraphTestDB", @"c:\temp\MMF");

            DB = new GraphDBEngine();
            IndexManager = new IndexFileManager(DB);
            DataManager = new DataFileManager(DB);
            TransactionManager = new TransactionFileManager(DB);
        }

        public void Dispose()
        {
            TransactionManager.Dispose();
            DataManager.Dispose();
            IndexManager.Dispose();
            DB.Dispose();
        }
    }
}
