// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Data.Entity;

namespace NakedFunctions.Rest.Test.Data {
    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
        public static string Server => LocalServer;
#endif

        public static readonly string CsMenu = @$"Data Source={Server};Initial Catalog={"MenuRestTests"};Integrated Security=True;";
        public static readonly string CsObject = @$"Data Source={Server};Initial Catalog={"ObjectRestTests"};Integrated Security=True;";
    }

    public class DatabaseInitializer<T> : DropCreateDatabaseAlways<T> where T : TestDbContext {
        protected override void Seed(T context) {
            // keep names 4 characters
            var fred = new SimpleRecord {Name = "Fred"};

            context.SimpleRecords.Add(fred);
            context.SimpleRecords.Add(new SimpleRecord {Name = "Bill"});
            context.SimpleRecords.Add(new SimpleRecord {Name = "Jack"});

            var ur = new UpdatedRecord {Name = ""};
            context.UpdatedRecords.Add(ur);

            var dr = new DateRecord();

            context.DateRecords.Add(dr);
            context.EnumRecords.Add(new EnumRecord());

            context.ReferenceRecords.Add(new ReferenceRecord {UpdatedRecord = ur, DateRecord = dr});

            //context.CollectionRecords.Add(new CollectionRecord {UpdatedRecords = new List<UpdatedRecord> {ur}});

            context.CollectionRecords.Add(new CollectionRecord {  });

            context.GuidRecords.Add(new GuidRecord());

            context.DisplayAsPropertyRecords.Add(new DisplayAsPropertyRecord());

            context.SaveChanges();
        }
    }

    public abstract class TestDbContext : DbContext {
        protected TestDbContext(string cs) : base(cs) { }

        public DbSet<SimpleRecord> SimpleRecords { get; set; }
        public DbSet<DateRecord> DateRecords { get; set; }
        public DbSet<EnumRecord> EnumRecords { get; set; }
        public DbSet<GuidRecord> GuidRecords { get; set; }
        public DbSet<ReferenceRecord> ReferenceRecords { get; set; }
        public DbSet<DisplayAsPropertyRecord> DisplayAsPropertyRecords { get; set; }
        public DbSet<UpdatedRecord> UpdatedRecords { get; set; }

        public DbSet<CollectionRecord> CollectionRecords { get; set; }


        protected void OnModelCreating<T>(DbModelBuilder modelBuilder) where T : TestDbContext {
            Database.SetInitializer(new DatabaseInitializer<T>());
        }
    }

    public class MenuDbContext : TestDbContext {
        public MenuDbContext() : base(Constants.CsMenu) { }
        public static void Delete() => Database.Delete(Constants.CsMenu);
        protected override void OnModelCreating(DbModelBuilder modelBuilder) => OnModelCreating<MenuDbContext>(modelBuilder);
    }

    public class ObjectDbContext : TestDbContext
    {
        public ObjectDbContext() : base(Constants.CsObject) { }
        public static void Delete() => Database.Delete(Constants.CsObject);
        protected override void OnModelCreating(DbModelBuilder modelBuilder) => OnModelCreating<ObjectDbContext>(modelBuilder);
    }
}