using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{
    public class DataStore2
    {
        private static DataStore2 _Internal;
        public static DataStore2 Use()
        {
            if (_Internal == null)
                _Internal = new DataStore2();

            return _Internal;
        }

        protected Entity[] Store;
        protected int NextIndex;

        public DataStore2()
        {
            NextIndex = 0;
            Store = new Entity[GraphDB.MAX_ITEMS];
        }

        public int Add(Entity obj)
        {
            int idx = NextIndex;
            Store[idx] = obj;
            NextIndex++;
            return idx;
        }
        public Entity Get(int index)
        {
            return Store[index];
        }
        public void Set(int index, Entity obj)
        {
            Store[index] = obj;
        }
        public void Remove(int index)
        {
            Store[index] = null;
        }

        public void Test()
        {

        }
    }


}
