// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.EntityFrameworkCore;

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

    public abstract class EFCoreTestDbContext : DbContext {
        private readonly string cs;

        protected EFCoreTestDbContext(string cs) => this.cs = cs;

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
        public DbSet<BoundedRecord> BoundedRecords { get; set; }
        public DbSet<ByteArrayRecord> ByteArrayRecords { get; set; }
        public DbSet<MaskRecord> MaskRecords { get; set; }
        public DbSet<HiddenRecord> HiddenRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var fred = new SimpleRecord {Id = 1, Name = "Fred"};

            modelBuilder.Entity<SimpleRecord>().HasData(fred);
            modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord {Id = 2, Name = "Bill"});
            modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord {Id = 3, Name = "Jack"});
            modelBuilder.Entity<SimpleRecord>().HasData(new SimpleRecord {Id = 4, Name = "hide it"});

            var ur = new UpdatedRecord {Id = 1, Name = ""};
            modelBuilder.Entity<UpdatedRecord>().HasData(ur);

            var dr = new DateRecord {Id = 1, EndDate = DateTime.Now, StartDate = DateTime.Now};

            modelBuilder.Entity<DateRecord>().HasData(dr);
            modelBuilder.Entity<EnumRecord>().HasData(new EnumRecord {Id = 1});

            modelBuilder.Entity<ReferenceRecord>().HasData(new {Id = 1, UpdatedRecordId = 1, DateRecordId = 1});

            //context.CollectionRecords.Add(new CollectionRecord {UpdatedRecords = new List<UpdatedRecord> {ur}});

            modelBuilder.Entity<CollectionRecord>().HasData(new CollectionRecord {Id = 1});

            modelBuilder.Entity<GuidRecord>().HasData(new GuidRecord {Id = 1});

            modelBuilder.Entity<DisplayAsPropertyRecord>().HasData(new DisplayAsPropertyRecord {Id = 1});

            modelBuilder.Entity<OrderedRecord>().HasData(new OrderedRecord {Id = 1});

            modelBuilder.Entity<EditRecord>().HasData(new {Id = 1, Name = "Jane", SimpleRecordId = 1, NotMatched = "no"});

            modelBuilder.Entity<DeleteRecord>().HasData(new DeleteRecord {Id = 1});
            modelBuilder.Entity<DeleteRecord>().HasData(new DeleteRecord {Id = 2});

            modelBuilder.Entity<BoundedRecord>().HasData(new BoundedRecord { Id = 1, Name = "One"});
            modelBuilder.Entity<BoundedRecord>().HasData(new BoundedRecord { Id = 2, Name = "Two"});

            modelBuilder.Entity<ByteArrayRecord>().HasData(new ByteArrayRecord() {Id = 1});

            modelBuilder.Entity<MaskRecord>().Ignore(m => m.MaskRecordProperty);
            modelBuilder.Entity<MaskRecord>().HasData(new MaskRecord { Id = 1, Name = "Title" });
            modelBuilder.Entity<HiddenRecord>().HasData(new MaskRecord { Id = 1, Name = "Title" });
        }
    }

    public class EFCoreMenuDbContext : EFCoreTestDbContext {
        public EFCoreMenuDbContext() : base(Constants.CsMenu) { }
        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();
    }

    public class EFCoreObjectDbContext : EFCoreTestDbContext {
        public EFCoreObjectDbContext() : base(Constants.CsObject) { }
        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();
    }
}