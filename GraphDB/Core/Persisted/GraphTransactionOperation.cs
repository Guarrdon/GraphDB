using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Transactions
{
    public class GraphTransactionOperation
    {
        public OperationType Operation { get; set; }
        public TransactionType Transaction { get; set; }

        public int Index { get; set; }
        public string EntityId { get; set; }

        public RelatedItem RelatedItem { get; set; }
        public GraphItem GraphItem { get; set; }
        public LookupItem LookupItem { get; set; }
        public Entity StoreItem { get; set; }
    }


}
