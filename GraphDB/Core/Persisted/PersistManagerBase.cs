using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public abstract class PersistManagerBase<TransactionT, ContainerT> : IDisposable
        where ContainerT : IEnumerable<TransactionT>, new()
    {
        protected ContainerT Container { get; set; }

        protected AutoResetEvent ExitHandle { get; set; }
        protected AutoResetEvent WaitHandle { get; set; }
        public bool AllowTransactionWriting;

        public string PersistedFile { get; set; }

        public PersistManagerBase()
        {
            Container = new ContainerT();

            AllowTransactionWriting = true;
            ExitHandle = new AutoResetEvent(false);
            WaitHandle = new AutoResetEvent(false);

            ProcessPersistRequests();
        }

        protected abstract void SetTransaction(ContainerT container, TransactionT transaction);
        protected virtual void ClearTransaction(ContainerT container, TransactionT transaction)
        {
            //if queue, nothing really has to happen here
        }
        protected abstract TransactionT GetTransaction(ContainerT container);

        public async void PersistTransaction(TransactionT transaction)
        {
            if (!AllowTransactionWriting)
                throw new ApplicationException("Cannot persist trasaction");

            await Task.Run(() =>
            {
                lock (Container)
                {
                    SetTransaction(Container, transaction);
                }
                WaitHandle.Set();
            });
        }
        protected async void ProcessPersistRequests()
        {
            await Task.Run(() =>
            {
                TransactionT transaction = default(TransactionT);

                int count = 0;
                do
                {
                    lock (Container)
                    {
                        if (Container.Count() > 0)
                            transaction = GetTransaction(Container);
                    }

                    if ((typeof(TransactionT).IsValueType && !transaction.Equals(default(TransactionT)) || transaction != null))
                    {
                        ProcessTransaction(transaction);
                    }

                    lock (Container)
                    {
                        ClearTransaction(Container, transaction);
                        count = Container.Count();
                    }

                    if (count == 0 && AllowTransactionWriting)
                        WaitHandle.WaitOne();

                } while (count > 0 || AllowTransactionWriting);

                ExitHandle.Set();
            });
        }

        protected virtual void ProcessTransaction(TransactionT transaction)
        {
            Serialize(transaction);
        }

        protected void Serialize<SerializeT>(SerializeT obj)
        {
            var itemdata = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None,
                                       new JsonSerializerSettings
                                       {
                                           NullValueHandling = NullValueHandling.Ignore
                                       });
            System.IO.File.AppendAllLines(PersistedFile, new string[] { itemdata });
        }

        public virtual void Dispose()
        {
            AllowTransactionWriting = false;
            WaitHandle.Set();
            ExitHandle.WaitOne();
        }
    }
}
