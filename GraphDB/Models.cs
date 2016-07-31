using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{
    public class Family : Entity
    {
        public string FamilyName { get; set; }
        public Family() : base() { }
    }

    public class Person : Entity
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person() : base() { }
    }
    public class Hair : Entity
    {
        public string Color { get; set; }
        public string Description { get; set; }

        public Hair() : base() { }
    }
}
