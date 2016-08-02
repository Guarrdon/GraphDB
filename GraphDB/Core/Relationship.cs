using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core
{
    public abstract class Relationship
    {

        public static Relationship Create(string typeName)
        {
            var relationship = (Relationship)Activator.CreateInstance(Type.GetType(typeName));
            relationship.Create();
            return relationship;
        }
        public static Relationship Create<RelationshipT>()
            where RelationshipT : Relationship, new()
        {
            var relationship = new RelationshipT();
            relationship.Create();
            return relationship;
        }


        public string Name { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityName { get; set; }
        public DirectionType EntityDirection { get; set; }
        public ConstraintType EntityConstraint { get; set; }
        public Relationship ReciprocalRelationship { get; set; }
        public bool IsReciprocal { get; set; }


        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(EntityTypeName)
                    && EntityDirection != DirectionType.None
                    && ReciprocalRelationship != null
                    && !string.IsNullOrWhiteSpace(ReciprocalRelationship.EntityTypeName)
                    && ReciprocalRelationship.EntityDirection != DirectionType.None;
            }
        }

        public Relationship()
        {
        }

        public abstract void Create(); 

        public  Relationship Build(string relationshipName)
        {
            Name = relationshipName;
            return this;
        }
      
        public Relationship Entity<EntityT>() => Entity<EntityT>(typeof(EntityT).FullName);
        public Relationship Entity<EntityT>(string name)
        {
            if (this.EntityTypeName == null)
            {
                //first use
                EntityTypeName = typeof(EntityT).FullName;
                EntityName = name;
            }
            else
            {
                //second use
                var self = this;
                ReciprocalRelationship = (Relationship)Activator.CreateInstance(this.GetType());

                ReciprocalRelationship.Name = Name;
                ReciprocalRelationship.EntityName = name;
                ReciprocalRelationship.EntityTypeName = typeof(EntityT).FullName;
                ReciprocalRelationship.EntityConstraint = EntityConstraint;
                ReciprocalRelationship.EntityDirection = EntityDirection.Opposite();
                ReciprocalRelationship.IsReciprocal = true;
                ReciprocalRelationship.ReciprocalRelationship = self;
               
            }
            return this;
        }
        public Relationship IsChildOf() => IsChildOf(ConstraintType.Mulitple);
        public Relationship IsChildOf(ConstraintType constraint)
        {
            EntityDirection = DirectionType.IsChildOf;
            EntityConstraint = constraint;
            return this;
        }
        public Relationship IsParentOf() => IsParentOf(ConstraintType.Mulitple);
        public Relationship IsParentOf(ConstraintType constraint)
        {
            EntityDirection = DirectionType.IsParentOf;
            EntityConstraint = constraint;
            return this;
        }
        public Relationship IsSiblingOf() => IsSiblingOf(ConstraintType.Mulitple);
        public Relationship IsSiblingOf(ConstraintType constraint)
        {
            EntityDirection = DirectionType.IsSiblingOf;
            EntityConstraint = constraint;
            return this;
        }
        public Relationship IsPropertyOf() => IsPropertyOf(ConstraintType.Mulitple);
        public Relationship IsPropertyOf(ConstraintType constraint)
        {
            EntityDirection = DirectionType.IsPropertyOf;
            EntityConstraint = constraint;
            return this;
        }
        public Relationship HasPropertyOf() => HasPropertyOf(ConstraintType.Mulitple);
        public Relationship HasPropertyOf(ConstraintType constraint)
        {
            EntityDirection = DirectionType.HasPropertyOf;
            EntityConstraint = constraint;
            return this;
        }



    }

    public class RelationshipContainer
    {
        public string RelationshipTypeName { get; set; }
        public bool Inverse { get; set; }
    }
}
