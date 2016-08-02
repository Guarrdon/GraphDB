using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class DataFileEnd
    {
        public int LastIndex { get; set; }
        public int MaxSize { get; set; }
    }
}
