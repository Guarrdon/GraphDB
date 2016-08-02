using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core.Persisted
{
    public class GraphDBData
    {
        public string DatabaseName { get; set; }
        public string DataFilePath { get; set; }
        public int MaxSize { get; set; }
      
    }
}
