using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{

    public enum DirectionType
    {
        None = 0,
        IsChildOf = 1,
        IsParentOf = 2,
        IsSiblingOf = 3,
        HasPropertyOf = 4,
        IsPropertyOf = 5
    }
    public enum ConstraintType
    {
        Mulitple = 0,
        Single = 1,
    }
    public class Entity
    {
        public string Id { get; set; }
        public Entity()
        {
        }
    }
 


}
