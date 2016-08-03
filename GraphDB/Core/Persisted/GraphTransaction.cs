using GraphDB.Core.Persisted;
using GraphDB.Core.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core
{
    public enum OperationType
    {
        None = 0,
        Merge = 1,
        Remove = 2
    }
  
    public enum TransactionType
    {
        None=0,
        GraphIndex = 1,
        StoreIndex = 2,
        LookupIndex = 3,
        RelatedIndex = 4,
        GraphItem=5 ,
        StoreItem=6,
        LookupItem=7,
        RelatedItem =8
    }
    public class GraphTransaction : IDisposable
    {

        public Queue<GraphTransactionOperation> Operations { get; set; }
        public GraphTransaction()
        {
            Operations = new Queue<GraphTransactionOperation>();
        }

        public void Remove(int index, TransactionType type)
        {
            Operations.Enqueue(new GraphTransactionOperation
            {
                Operation = OperationType.Remove,
                Transaction = type,
                Index = index
            });
        }
        public void RemoveLookup(string entityId)
        {
            Operations.Enqueue(new GraphTransactionOperation
            {
                Operation = OperationType.Remove,
                Transaction = TransactionType.LookupIndex,
                EntityId = entityId
            });
        }



        public void Dispose()
        {
        }
    }

  
  
}
