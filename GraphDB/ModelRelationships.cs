using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{
    public class IsFamilyOf : Relationship
    {
        public IsFamilyOf()
        {

        }
        public override void Create()
        {
            Build("IsFamilyOf").Entity<Family>().IsParentOf().Entity<Person>();
        }
    }
    public class IsParentOf : Relationship
    {
        public IsParentOf()
        {

        }
        public override void Create()
        {
            Build("IsParentOf").Entity<Person>("Parent").IsParentOf().Entity<Person>("Child");
        }
    }
    public class IsSpouseOf : Relationship
    {
        public IsSpouseOf()
        {

        }
        public override void Create()
        {
            Build("IsSpouseOf").Entity<Person>("Spouse").IsSiblingOf(ConstraintType.Single).Entity<Person>();
        }
    }
    public class IsSisterOf : Relationship
    {
        public IsSisterOf()
        {

        }
        public override void Create()
        {
            Build("IsSisterOf").Entity<Person>("Sister").IsSiblingOf().Entity<Person>();
        }
    }
    public class IsBrotherOf : Relationship
    {
        public override void Create()
        {
            Build("IsBrotherOf").Entity<Person>("Brother").IsSiblingOf().Entity<Person>();
        }
    }
    public class HasHairOf : Relationship
    {
        public HasHairOf()
        {

        }
        public override void Create()
        {
            Build("HasHairOf").Entity<Person>().HasPropertyOf(ConstraintType.Single).Entity<Hair>();
        }
    }


}
