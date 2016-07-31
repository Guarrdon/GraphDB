using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{


    public class IndexItem
    {
        public string EntityID { get; set; }
        public int StoreId { get; set; }
        public string EntityTypeName { get; set; }

        public HashSet<RelatedItem> Related { get; set; }

        public IndexItem()
        {
            Related = new HashSet<RelatedItem>();
        }
    }
    public struct RelatedItem
    {
        public int Index;
        public string Relationship;
        public bool Inverse;
    }
}
