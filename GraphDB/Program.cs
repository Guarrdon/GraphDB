using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphDB.Core;
using GraphDB.UserDefined;
using GraphDB.Core.Persisted;
using GraphDB.Core.Transactions;

namespace GraphDB
{
    class Program
    {
        static void Main(string[] args)
        {
            DataFileManager.Use("GraphTestDB", @"c:\temp\MMF");
            var db = new GraphDBEngine();
            var fam = new Graph(new Family { FamilyName = "Test Family" });
            for (int i=0; i<10000; i++)
            {
                fam.Add<IsFamilyOf>(new Graph(new Person { Name = $"Name:{i}" }));
            }
            db.Merge(fam);

            var x1 = db.Get("GraphDB.UserDefined.Person|2001");
            db.Remove(x1);
            Console.ReadKey();

            var x2 = db.Get("GraphDB.UserDefined.Person|4000");
            db.Remove(x2);
            Console.ReadKey();


            var x3 = db.Get("GraphDB.UserDefined.Person|6000");
            db.Remove(x3);
            Console.ReadKey();


            GraphTransactionManager.Use().Dispose();
        }

        public static void Test1()
        {
            //Build some test data entities
            //Keep only one entity (never copies)
            //Variables are just useful for reference
            var f1 = new Graph(new Family { FamilyName = "Lyons Clan" });
            var f2 = new Graph(new Family { FamilyName = "Marissa's Family" });
            var a1 = new Graph(new Person { Name = "Mom", Age = 61 });
            var a2 = new Graph(new Person { Name = "Dad", Age = 63 });
            var b1 = new Graph(new Person { Name = "Matt", Age = 38 });
            var p1 = new Graph(new Person { Name = "Kevin", Age = 38 });
            var p2 = new Graph(new Person { Name = "Marissa", Age = 36 });
            var p3 = new Graph(new Person { Name = "Mikayla", Age = 17 });
            var p4 = new Graph(new Person { Name = "Madalyn", Age = 11 });
            var p5 = new Graph(new Person { Name = "Draven", Age = 10 });
            var p6 = new Graph(new Person { Name = "Miranda", Age = 9 });
            var p7 = new Graph(new Person { Name = "Melodee", Age = 4 });
            var h0 = new Graph(new Hair { Color = "None", Description = "Bald" });
            var h1 = new Graph(new Hair { Color = "Brunette", Description = "Short" });
            var h2 = new Graph(new Hair { Color = "Blonde", Description = "Long" });
            var h3 = new Graph(new Hair { Color = "Blonde", Description = "Curly Long" });

            //Relate those entities with predefined relationships (from ModelRelationships file)
            //entity    is related to   entities
            //a2        IsParentOf      p2 and b1
            f1.Add<IsFamilyOf>(a1, a2, b1, p2, p3, p4, p5, p6, p7);
            f2.Add<IsFamilyOf>(p1, p2, p3, p4, p5, p6, p7);
            a1.Add<IsParentOf>(p2, b1);
            a2.Add<IsParentOf>(p2, b1);
            b1.Add<IsBrotherOf>(p2)
                .Add<HasHairOf>(h0);
            p1.Add<IsSpouseOf>(p2);
            p1.Add<IsParentOf>(p3, p4, p5, p6, p7)
                .Add<HasHairOf>(h1);
            p2.Add<IsParentOf>(p3, p4, p5, p6, p7)
                .Add<IsSisterOf>(b1)
                .Add<HasHairOf>(h2);
            p3.Add<IsSisterOf>(p4, p5, p6, p7)
                .Add<HasHairOf>(h3);
            p4.Add<IsSisterOf>(p5, p6, p7)
                .Add<HasHairOf>(h2);
            p5.Add<IsBrotherOf>(p3, p4, p5, p6, p7)
                .Add<HasHairOf>(h1);
            p6.Add<IsSisterOf>(p7)
                .Add<HasHairOf>(h3);
            p7.Add<HasHairOf>(h3);

            //Create new GraphDB object and merge data into db
            var db = new GraphDBEngine();
            db.Merge(f1);
            

            //Data is now queryable
            //StartWith(1) and then (Filter(1-N) or Related(1-N))
            //Returns IEnumerable<Graph> to work with
            var query1 = db.Get(new Filter()
                .StartWith(p2)
                .Related<IsParentOf>(true));
            var query2 = db.Get(new Filter()
                .StartWith<Hair>()
                .Where<Hair>(x => x.Color == "Blonde")
                .Related<HasHairOf>(true)
                .Related<IsParentOf>(true));
            var query3 = db.Get(new Filter()
                .StartWith<Hair>()
                .Where<Hair>(x => x.Color == "None")
                .Related<HasHairOf>(true)
                .Related<IsBrotherOf>());
        }


    }

}

