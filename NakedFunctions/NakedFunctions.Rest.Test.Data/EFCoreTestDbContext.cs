// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Protocols;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace NakedFunctions.Rest.Test.Data {
    public static class EFCoreConstants {
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

    //public class EFCoreDatabaseInitializer {
    //    protected override void Seed(T context) {
    //        // keep names 4 characters
    //        var fred = new SimpleRecord {Name = "Fred"};

    //        context.SimpleRecords.Add(fred);
    //        context.SimpleRecords.Add(new SimpleRecord {Name = "Bill"});
    //        context.SimpleRecords.Add(new SimpleRecord {Name = "Jack"});
    //        context.SimpleRecords.Add(new SimpleRecord {Name = "hide it"});

    //        var ur = new UpdatedRecord {Name = ""};
    //        context.UpdatedRecords.Add(ur);

    //        var dr = new DateRecord();

    //        context.DateRecords.Add(dr);
    //        context.EnumRecords.Add(new EnumRecord());

    //        context.ReferenceRecords.Add(new ReferenceRecord {UpdatedRecord = ur, DateRecord = dr});

    //        //context.CollectionRecords.Add(new CollectionRecord {UpdatedRecords = new List<UpdatedRecord> {ur}});

    //        context.CollectionRecords.Add(new CollectionRecord());

    //        context.GuidRecords.Add(new GuidRecord());

    //        context.DisplayAsPropertyRecords.Add(new DisplayAsPropertyRecord());

    //        context.OrderedRecords.Add(new OrderedRecord());

    //        context.EditRecords.Add(new EditRecord {Name = "Jane", SimpleRecord = fred, NotMatched = "no"});

    //        context.DeleteRecords.Add(new DeleteRecord());
    //        context.DeleteRecords.Add(new DeleteRecord());

    //        context.SaveChanges();
    //    }
    //}

    public abstract class EFCoreTestDbContext : DbContext {
        private readonly string cs;
        protected EFCoreTestDbContext(string cs) {
            this.cs = cs;
        }

        public DbSet<SimpleRecord> SimpleRecords { get; set; }
        public DbSet<DateRecord> DateRecords { get; set; }
        public DbSet<EnumRecord> EnumRecords { get; set; }
        public DbSet<GuidRecord> GuidRecords { get; set; }
        public DbSet<ReferenceRecord> ReferenceRecords { get; set; }
        public DbSet<DisplayAsPropertyRecord> DisplayAsPropertyRecords { get; set; }
        public DbSet<UpdatedRecord> UpdatedRecords { get; set; }
        public DbSet<CollectionRecord> CollectionRecords { get; set; }
        public DbSet<OrderedRecord> OrderedRecords { get; set; }
        public DbSet<EditRecord> EditRecords { get; set; }
        public DbSet<DeleteRecord> DeleteRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(cs);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var fred = new SimpleRecord { Id = 1,  Name = "Fred" };

            modelBuilder.Entity<SimpleRecord>().HasData(fred);
            modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord { Id = 2, Name = "Bill" });
            modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord { Id = 3, Name = "Jack" });
            modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord { Id = 4, Name = "hide it" });

            //var ur = new UpdatedRecord { Name = "" };
            //modelBuilder.Entity<UpdatedRecord>().HasData(ur);

            //var dr = new DateRecord();

            //modelBuilder.Entity<DateRecord>().HasData(dr);
            //modelBuilder.Entity<EnumRecord>().HasData(new EnumRecord());

            //modelBuilder.Entity<ReferenceRecord>().HasData(new ReferenceRecord { UpdatedRecord = ur, DateRecord = dr });

            ////context.CollectionRecords.Add(new CollectionRecord {UpdatedRecords = new List<UpdatedRecord> {ur}});

            //modelBuilder.Entity<CollectionRecord>().HasData(new CollectionRecord());

            //modelBuilder.Entity<GuidRecord>().HasData(new GuidRecord());

            //modelBuilder.Entity<DisplayAsPropertyRecord>().HasData(new DisplayAsPropertyRecord());

            //modelBuilder.Entity<OrderedRecord>().HasData(new OrderedRecord());

            //modelBuilder.Entity<EditRecord>().HasData(new EditRecord { Name = "Jane", SimpleRecord = fred, NotMatched = "no" });

            //modelBuilder.Entity<DeleteRecord>().HasData(new DeleteRecord());
            //modelBuilder.Entity<DeleteRecord>().HasData(new DeleteRecord());


        }
    }

    public class EFCoreMenuDbContext : EFCoreTestDbContext
    {
        public EFCoreMenuDbContext() : base(Constants.CsMenu) { }
        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();
        //protected override void OnModelCreating(ModelBuilder modelBuilder) => OnModelCreating<MenuDbContext>(modelBuilder);
    }

    public class EFCoreObjectDbContext : EFCoreTestDbContext
    {
        public EFCoreObjectDbContext() : base(Constants.CsObject) { }
        public void Delete() => Database.EnsureDeleted();
        public void Create() => Database.EnsureCreated();
        //protected override void OnModelCreating(ModelBuilder modelBuilder) => OnModelCreating<ObjectDbContext>(modelBuilder);
    }
}