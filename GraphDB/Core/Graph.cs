using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB.Core
{

    public class Graph
    {
        public Entity CurrentEntity { get; set; }
        public EntityT GetEntity<EntityT>()
            where EntityT : Entity
        {
            return CurrentEntity as EntityT;
        }

        public string TypeName
        {
            get
            {
                if (CurrentEntity != null)
                {
                    return CurrentEntity.GetType().FullName;
                }
                return "";
            }
        }

        public List<RelationshipGraph> Parents { get; set; }
        public List<RelationshipGraph> Children { get; set; }
        public List<RelationshipGraph> Siblings { get; set; }
        public List<RelationshipGraph> Properties { get; set; }
        public List<RelationshipGraph> Owners { get; set; }


        public Graph(Entity entity)
        {
            CurrentEntity = entity;
            Parents = new List<RelationshipGraph>();
            Children = new List<RelationshipGraph>();
            Siblings = new List<RelationshipGraph>();
            Properties = new List<RelationshipGraph>();
            Owners = new List<RelationshipGraph>();
        }

        public Graph Add<RelationshipT>(Graph entity)
            where RelationshipT : Relationship, new()
                => Add<RelationshipT>(entity, false, true);
        public Graph Add<RelationshipT>(Graph entity, bool inverse)
            where RelationshipT : Relationship, new()
                => Add<RelationshipT>( entity, inverse, true);
        public Graph Add<RelationshipT>(Graph entity, bool inverse, bool doRelated)
            where RelationshipT : Relationship, new()
                => Add(Relationship.Create<RelationshipT>(), entity, inverse, doRelated);
        public Graph Add(Relationship relationship, Graph entity, bool inverse, bool doRelated)
        {
            if (inverse)
                relationship = relationship.ReciprocalRelationship;

            if (CurrentEntity == null)
                throw new Exception("No source entity to add to");
            if (relationship == null)
                throw new ArgumentNullException("No relationship to build graph from");

            if (relationship.EntityDirection == DirectionType.IsChildOf)
            {
                if (relationship.EntityConstraint == ConstraintType.Single && Parents.Any(x => x.Relationship.Name == relationship.Name))
                    throw new Exception("Constraint violated.  Cannot add more entities");
                Parents.Add(new RelationshipGraph { Relationship = relationship, Graph = entity });
            }
            if (relationship.EntityDirection == DirectionType.IsParentOf)
            {
                if (relationship.EntityConstraint == ConstraintType.Single && Children.Any(x => x.Relationship.Name == relationship.Name))
                    throw new Exception("Constraint violated.  Cannot add more entities");
                Children.Add(new RelationshipGraph { Relationship = relationship, Graph = entity });
            }
            if (relationship.EntityDirection == DirectionType.IsSiblingOf)
            {
                if (relationship.EntityConstraint == ConstraintType.Single && Siblings.Any(x => x.Relationship.Name==relationship.Name))
                    throw new Exception("Constraint violated.  Cannot add more entities");
                Siblings.Add(new RelationshipGraph { Relationship = relationship, Graph = entity });
            }
            if (relationship.EntityDirection == DirectionType.HasPropertyOf)
            {
                if (relationship.EntityConstraint == ConstraintType.Single && Properties.Any(x => x.Relationship.Name == relationship.Name))
                    throw new Exception("Constraint violated.  Cannot add more entities");
                Properties.Add(new RelationshipGraph { Relationship = relationship, Graph = entity });
            }
            if (relationship.EntityDirection == DirectionType.IsPropertyOf)
            {
                //Properties must be able to be owned by multiple people...cannot lock it to single constraint.
                //if (relationship.EntityConstraint == ConstraintType.Single && Owners.Any(x => x.Relationship.Name == relationship.Name))
                //    throw new Exception("Constraint violated.  Cannot add more entities");
                Owners.Add(new RelationshipGraph { Relationship = relationship, Graph = entity });
            }

            if (doRelated)
                entity.Add(relationship, this, true, false);

            return this;
        }

        public Graph Add<RelationshipT>(params Graph[] entities)
            where RelationshipT : Relationship, new()
                => Add<RelationshipT>(false, entities);
        public Graph Add<RelationshipT>(bool inverse, params Graph[] entities)
            where RelationshipT : Relationship, new()
        {
            foreach (Graph entity in entities)
                this.Add<RelationshipT>(entity, inverse);

            return this;
        }

    }

    public class RelationshipGraph
    {
        public Relationship Relationship { get; set; }
        public Graph Graph { get; set; }
    }
}
