using GraphDB.Core.Persisted;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphDB.Core.Transactions
{
    public class GraphTransactionManager : IDisposable
    {
        private static GraphTransactionManager _Internal;
        public static GraphTransactionManager Use()
        {
            if (_Internal == null)
            {
                _Internal = new GraphTransactionManager
                {
                    TransactionFile = DataFileManager.Use().TransactionFile
                };
            }

            return _Internal;
        }

        public string TransactionFile { get; set; }
        protected Queue<Queue<GraphTransactionOperation>> Transactions { get; set; }

        private AutoResetEvent _ExitHandle;
        private AutoResetEvent _WaitHandle;
        private bool _AllowTransactionWriting;
        public GraphTransactionManager()
        {
            _AllowTransactionWriting = true;
            Transactions = new Queue<Queue<GraphTransactionOperation>>();
            _ExitHandle = new AutoResetEvent(false);
            _WaitHandle = new AutoResetEvent(false);
            TryWriteTransactions();
        }

        public async void PersistTransaction(Queue<GraphTransactionOperation> operations)
        {
            if (!_AllowTransactionWriting)
                throw new ApplicationException("Cannot persist trasaction");

            await Task.Run(() =>
            {
                lock (Transactions)
                {
                    Transactions.Enqueue(operations);
                }
                _WaitHandle.Set();
            });
        }

        public async void TryWriteTransactions()
        {
            await Task.Run(() =>
            {
                Queue<GraphTransactionOperation> GraphTransaction = null;
                int count = 0;
                do
                {
                    lock (Transactions)
                    {
                        if (Transactions.Count > 0)
                            GraphTransaction = Transactions.Dequeue();
                    }

                    if (GraphTransaction != null)
                    {
                        while (GraphTransaction.Count > 0)
                        {
                            var item = GraphTransaction.Dequeue();
                            var itemdata = JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.None,
                                new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore
                                });
                            System.IO.File.AppendAllLines(TransactionFile, new string[] { itemdata });
                        }
                    }

                    lock (Transactions)
                    {
                        count = Transactions.Count;
                    }

                    if (count == 0 && _AllowTransactionWriting)
                        _WaitHandle.WaitOne();

                } while (count > 0 || _AllowTransactionWriting);

                _ExitHandle.Set();
            });
        }

        public void Dispose()
        {
            _AllowTransactionWriting = false;
            _WaitHandle.Set();
            _ExitHandle.WaitOne();
        }

    }

}
