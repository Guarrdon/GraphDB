using GraphDB.Core.Persisted;
using GraphDB.Core.Transactions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class TransactionFileManager : PersistManagerBase<Queue<GraphTransactionOperation>, Queue<Queue<GraphTransactionOperation>>>
    {
        public GraphDBEngine DB { get; set; }

        public TransactionFileManager(GraphDBEngine db) : base()
        {
            PersistedFile = $"{AppSettings.Use().DatabaseName}.xaction";
            DB = db;
            DB.ApplyTransaction += PersistTransaction;
        }

        protected override void SetTransaction(Queue<Queue<GraphTransactionOperation>> container, Queue<GraphTransactionOperation> transaction)
        {
            container.Enqueue(transaction);
        }

        protected override Queue<GraphTransactionOperation> GetTransaction(Queue<Queue<GraphTransactionOperation>> container)
        {
            return container.Dequeue();
        }

        public override void Dispose()
        {
            DB.ApplyTransaction -= PersistTransaction;
            base.Dispose();
        }
    }

}
