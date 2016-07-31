using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{
    public enum FilterOperation
    {
        StartEntityType = 0,
        StartObject = 1,
        StartGraph = 2,
        StartAll = 3,
        Where = 4,
        Related = 5
    }

    public class Filter
    {
        public string StartEntityTypeName { get; set; }
        public string StartObjectId { get; set; }
        public Graph StartGraph { get; set; }

        public Queue<LambdaExpression> WhereClauses;
        public Queue<RelationshipContainer> RelatedClauses;

        public Delegate SelectClause;

        public Queue<FilterOperation> OrderOfOperations;
        public Filter()
        {
            OrderOfOperations = new Queue<FilterOperation>();

            WhereClauses = new Queue<LambdaExpression>();
            RelatedClauses = new Queue<RelationshipContainer>();
        }


        public Filter StartWith<EntityT>()
        {
            StartEntityTypeName = typeof(EntityT).FullName;
            OrderOfOperations.Enqueue(FilterOperation.StartEntityType);
            return this;
        }
        public Filter StartWith(string objectId)
        {
            StartObjectId = objectId;
            OrderOfOperations.Enqueue(FilterOperation.StartObject);
            return this;
        }
        public Filter StartWith(Graph graph)
        {
            StartGraph = graph;
            OrderOfOperations.Enqueue(FilterOperation.StartGraph);
            return this;
        }
        public Filter StartWith()
        {
            OrderOfOperations.Enqueue(FilterOperation.StartAll);
            return this;
        }

        public Filter Where<EntityT>(Expression<Func<EntityT, bool>> expr)
            where EntityT : Entity
        {
            WhereClauses.Enqueue(expr);
            OrderOfOperations.Enqueue(FilterOperation.Where);
            return this;
        }

        public Filter Related<RelationshipT>()
            where RelationshipT : Relationship, new()
                => Related<RelationshipT>(false);
        public Filter Related<RelationshipT>(bool inverse)
            where RelationshipT:Relationship, new()
        {
            var relationship = new RelationshipT();

            RelatedClauses.Enqueue(new RelationshipContainer { RelationshipTypeName = relationship.GetType().FullName, Inverse = inverse });
            OrderOfOperations.Enqueue(FilterOperation.Related);
            return this;
        }



    }
   
}
