using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class IndexFileManager : PersistManagerBase<int, HashSet<int>>
    {

        public GraphDBEngine DB { get; set; }
        public IndexFileManager(GraphDBEngine db) : base()
        {
             DB = db;
           DB.ApplyIndex += PersistTransaction;
        }
        protected override void SetTransaction(HashSet<int> container, int transaction)
        {
            container.Add(transaction / AppSettings.DATABLOCKSIZE);
        }

        protected override int GetTransaction(HashSet<int> container)
        {
            return container.First();
        }

        protected override void ClearTransaction(HashSet<int> container, int transaction)
        {
            container.Remove(transaction);
        }

        protected override void ProcessTransaction(int transaction)
        {
            Console.WriteLine($"Index Transaction - {transaction}");


            //Serialize(transaction);
        }

        public override void Dispose()
        {
            DB.ApplyIndex -= PersistTransaction;
            base.Dispose();
        }
    }
}
