﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDBTest
{
    public class GraphDB
    {
        public const int MAX_ITEMS = 10000000; //10,000,000

        protected IndexItem[] Items;
        protected Dictionary<string, int> ItemIndex { get; set; }

        protected int NextItem;

        /// <summary>
        /// constructor
        /// </summary>
        public GraphDB()
        {
            Items = new IndexItem[MAX_ITEMS];
            ItemIndex = new Dictionary<string, int>();

            NextItem = 0;
        }

        /// <summary>
        /// Merge - Add and Modify (no delete)
        /// </summary>
        /// <param name="graph"></param>
        public void Merge(Graph graph)
        {
            var mergeOperations = new HashSet<string>();
            MergeInternal(graph, ref mergeOperations);
            mergeOperations = null;
        }
        protected void MergeInternal(Graph graph, ref HashSet<string> mergeOperations)
        {
            if (graph.CurrentEntity.Id == null || !mergeOperations.Contains(graph.CurrentEntity.Id))
            {
                if (graph.CurrentEntity.Id == null)
                {
                    graph.CurrentEntity.Id = $"{graph.TypeName}|{NextItem}";
                    Items[NextItem] = new IndexItem();
                    Items[NextItem].EntityID = graph.CurrentEntity.Id;
                    Items[NextItem].EntityTypeName = graph.TypeName;
                    Items[NextItem].StoreId = DataStore2.Use().Add(graph.CurrentEntity);
                    ItemIndex.Add(graph.CurrentEntity.Id, NextItem);

                    NextItem++;
                }
                else
                {
                    DataStore2.Use().Set(Items[ItemIndex[graph.CurrentEntity.Id]].StoreId, graph.CurrentEntity);
                }
                mergeOperations.Add(graph.CurrentEntity.Id);

                MergeInternalRelated(graph, ref mergeOperations);
            }
        }
        protected void MergeInternalRelated(Graph graph, ref HashSet<string> mergeOperations)
        {
            BuildRelated(graph.CurrentEntity.Id, graph.Children, ref mergeOperations);
            BuildRelated(graph.CurrentEntity.Id, graph.Parents, ref mergeOperations);
            BuildRelated(graph.CurrentEntity.Id, graph.Siblings, ref mergeOperations);
            BuildRelated(graph.CurrentEntity.Id, graph.Properties, ref mergeOperations);
            BuildRelated(graph.CurrentEntity.Id, graph.Owners, ref mergeOperations);
        }
        private void BuildRelated(string entityId, List<RelationshipGraph> related, ref HashSet<string> mergeOperations)
        {
            foreach (RelationshipGraph x in related)
            {
                MergeInternal(x.Graph, ref mergeOperations);
                var relatedSet = Items[ItemIndex[entityId]].Related;
                relatedSet.Add(new RelatedItem
                {
                    Index = ItemIndex[x.Graph.CurrentEntity.Id],
                    Relationship = x.Relationship.GetType().FullName,
                    Inverse = x.Relationship.IsReciprocal
                });
            }
        }


        /// <summary>
        /// Get - Limits hierarchy returned to immediate relationships only
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="limitHierarchy"></param>
        /// <returns></returns>
        public Graph Get(string objectId, bool limitHierarchy)
        {
            if (limitHierarchy)
            {
                return GetInternal(objectId, ref limitHierarchy);
            }
            else
                return Get(objectId);
        }
        protected Graph GetInternal(string objectId, ref bool limitHierarchy)
        {
            if (ItemIndex.ContainsKey(objectId))
            {
                var item = Items[ItemIndex[objectId]];
                var entity = DataStore2.Use().Get(item.StoreId);
                var graph = new Graph(entity);

                if (limitHierarchy)
                {
                    limitHierarchy = false;
                    GetInternalRelated(graph, ref limitHierarchy);
                }
                return graph;
            }
            return null;
        }
        protected Graph GetInternalLimited(int index)
        {
            var item = Items[index];
            var entity = DataStore2.Use().Get(item.StoreId);
            var graph = new Graph(entity);

            var limit = false;
            GetInternalRelated(graph, ref limit);

            return graph;
        }
        protected void GetInternalRelated(Graph graph, ref bool limitHierarchy)
        {
            var mainItem = Items[ItemIndex[graph.CurrentEntity.Id]];
            foreach (RelatedItem x in mainItem.Related)
            {
                var item = GetInternal(Items[x.Index].EntityID, ref limitHierarchy);
                var relationship = Relationship.Create(x.Relationship);
                graph.Add(relationship, item, x.Inverse, false);
            }
        }
        /// <summary>
        /// Get - Complete hierarchy (use at your own risk)
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public Graph Get(string objectId)
        {
            var mergeOperations = new Dictionary<string, Graph>();
            var graph = GetInternal(objectId, ref mergeOperations);
            mergeOperations = null;
            return graph;
        }
        protected Graph GetInternal(string objectId, ref Dictionary<string, Graph> mergeOperations)
        {
            if (ItemIndex.ContainsKey(objectId))
            {
                if (!mergeOperations.ContainsKey(objectId))
                {
                    var item = Items[ItemIndex[objectId]];
                    var entity = DataStore2.Use().Get(item.StoreId);
                    var graph = new Graph(entity);
                    mergeOperations.Add(objectId, graph);

                    GetInternalRelated(graph, ref mergeOperations);
                }
                return mergeOperations[objectId];
            }
            return null;
        }
        protected void GetInternalRelated(Graph graph, ref Dictionary<string, Graph> mergeOperations)
        {
            var mainItem = Items[ItemIndex[graph.CurrentEntity.Id]];
            foreach (RelatedItem x in mainItem.Related)
            {
                var item = GetInternal(Items[x.Index].EntityID, ref mergeOperations);
                var relationship = Relationship.Create(x.Relationship);
                graph.Add(relationship, item, x.Inverse, false);
            }
        }
        /// <summary>
        /// Removes nodes from graph database and deletes data in data store
        /// Will only remove primary node in graph and all relationships with ONLY that primary node
        /// </summary>
        /// <param name="graph"></param>
        public void Remove(Graph graph)
        {
            var id = ItemIndex[graph.CurrentEntity.Id];
            var item = Items[id];
            foreach (RelatedItem r in item.Related)
            {
                var relateditem = Items[r.Index];
                relateditem.Related.RemoveWhere(x => x.Index == id);
            }
            DataStore2.Use().Remove(item.StoreId);
            ItemIndex.Remove(item.EntityID);
            Items[id] = null;
        }


        public IEnumerable<Graph> Get(Filter filter)
        {
            var indexes = new HashSet<int>();

            while (filter.OrderOfOperations.Count > 0)
            {
                var operation = filter.OrderOfOperations.Dequeue();

                ////Filter by object (fast!!!!)
                if (operation == FilterOperation.StartObject)
                {
                    indexes.Add(ItemIndex[filter.StartObjectId]);
                }
                ////Filter by graph (fast!!!!)
                else if (operation == FilterOperation.StartGraph)
                {
                    indexes.Add(ItemIndex[filter.StartGraph.CurrentEntity.Id]);
                }
                ////Filter by type (slow-->medium!!!)
                else if (operation == FilterOperation.StartEntityType)
                {
                    foreach (int index in ItemIndex.Values)
                        if (Items[index].EntityTypeName == filter.StartEntityTypeName)
                            indexes.Add(index);
                }
                ////Filter by all (very slow!!!!)
                else if (operation == FilterOperation.StartAll)
                {
                    indexes = new HashSet<int>(ItemIndex.Values);
                }
                ////Where filter operations (slow!!!)
                else if (operation == FilterOperation.Where)
                {
                    var newIndexes = new HashSet<int>();
                    var where = filter.WhereClauses.Dequeue();
                    foreach (int idx in indexes)
                    {
                        var obj = DataStore2.Use().Get(Items[idx].StoreId);
                        var match = (bool)where.Compile().DynamicInvoke(obj);
                        if (match)
                            newIndexes.Add(idx);
                    }
                    indexes = newIndexes;
                }
                ////Navigate relationship tree (fast!!!)
                else if (operation == FilterOperation.Related)
                {
                    var newIndexes = new HashSet<int>();
                    var container = filter.RelatedClauses.Dequeue();
                    var relationship = Relationship.Create(container.RelationshipTypeName);
                    var inverse = container.Inverse;
                    IndexItem item = null;
                    foreach (int idx in indexes)
                    {
                        item = Items[idx];
                        foreach (RelatedItem related in item.Related)
                        {
                            if (related.Inverse == inverse && related.Relationship == relationship.GetType().FullName)
                                newIndexes.Add(related.Index);
                        }
                    }
                    indexes = newIndexes;
                }
            }

            return indexes.Select(x => GetInternalLimited(x));
        }


        public void Test()
        {
            DataStore2.Use().Test();
        }

    }


}
