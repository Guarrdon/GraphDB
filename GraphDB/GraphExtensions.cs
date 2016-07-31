using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{
    public static class GraphExtensions
    {
        public static DirectionType Opposite(this DirectionType direction)
        {
            if (direction == DirectionType.IsChildOf)
                return DirectionType.IsParentOf;
            if (direction == DirectionType.IsParentOf)
                return DirectionType.IsChildOf;
            if (direction == DirectionType.IsPropertyOf)
                return DirectionType.HasPropertyOf;
            if (direction == DirectionType.HasPropertyOf)
                return DirectionType.IsPropertyOf;


            return direction;
        }
    }
}
