// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.EntityFrameworkCore;

namespace Legacy.Rest.Test.Data {
    public static class Constants {
        public static string AppveyorServer => @"(local)\SQL2017";
        public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
        public static string Server => LocalServer;
#endif

        public static readonly string CsLegacy = @$"Data Source={Server};Initial Catalog={"LegacyTests"};Integrated Security=True;";
    }

    public abstract class EFCoreTestDbContext : DbContext {
        private readonly string cs;

        protected EFCoreTestDbContext(string cs) => this.cs = cs;

        public DbSet<ClassWithTextString> ClassesWithTextString { get; set; }
        public DbSet<ClassWithInternalCollection> ClassesWithInternalCollection { get; set; }
        public DbSet<ClassWithActionAbout> ClassesWithActionAbout { get; set; }
        public DbSet<ClassWithFieldAbout> ClassesWithFieldAbout { get; set; }
        public DbSet<ClassWithString> ClassesWithString { get; set; }
        public DbSet<ClassWithLinkToNOFClass> ClassesWithLinkToNOFClass { get; set; }
        public DbSet<ClassWithNOFInternalCollection> ClassesWithNOFInternalCollection { get; set; }
        public DbSet<LegacyClassWithInterface> LegacyClassWithInterfaces { get; set; }
        public DbSet<ClassWithLegacyInterface> ClassWithLegacyInterfaces { get; set; }
        public DbSet<ClassWithMenu> ClassWithMenus { get; set; }
        public DbSet<ClassWithDate> ClassWithDates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            var fred = new { Id = 1, name = "Fred", ClassWithInternalCollectionId = 1, ClassWithStringId = 2 };
            var bill = new { Id = 2, name = "Bill", ClassWithStringId = 2 };
            var jane = new { Id = 1, date = new DateTime(2021, 11, 1) };

            modelBuilder.Entity<ClassWithTextString>().Ignore(t => t.Name);
            modelBuilder.Entity<ClassWithTextString>().Property("name").HasColumnName("Name");

            modelBuilder.Entity<ClassWithDate>().Ignore(t => t.Date);
            modelBuilder.Entity<ClassWithDate>().Property("date").HasColumnName("Date");



            modelBuilder.Entity<ClassWithInternalCollection>().Ignore(t => t.TestCollection);

            modelBuilder.Entity<ClassWithInternalCollection>()
                        .HasMany(c => c._TestCollection);

            modelBuilder.Entity<ClassWithInternalCollection>().HasData(new ClassWithInternalCollection { Id = 1 });
            modelBuilder.Entity<ClassWithInternalCollection>().HasData(new ClassWithInternalCollection { Id = 2 });

            modelBuilder.Entity<ClassWithNOFInternalCollection>().HasData(new { Id = 1 });

            modelBuilder.Entity<ClassWithString>().HasData(new { Id = 2, Name = "Ted", ClassWithNOFInternalCollectionId = 1 });

            modelBuilder.Entity<ClassWithTextString>().HasData(fred);
            modelBuilder.Entity<ClassWithTextString>().HasData(bill);

            modelBuilder.Entity<ClassWithActionAbout>().HasData(new  { Id = 1 });
            modelBuilder.Entity<ClassWithFieldAbout>().HasData(new  { Id = 1 });
            modelBuilder.Entity<ClassWithMenu>().HasData(new { Id = 1 });
            modelBuilder.Entity<ClassWithDate>().HasData(jane);

            modelBuilder.Entity<LegacyClassWithInterface>().HasData(new  { Id = 10 });
            modelBuilder.Entity<ClassWithLegacyInterface>().HasData(new  { Id = 10 });

            modelBuilder.Entity<ClassWithString>().HasData(new { Id = 1, Name = "Jill", LinkToLegacyClassId = 1, ClassWithNOFInternalCollectionId = 1 });

            modelBuilder.Entity<ClassWithLinkToNOFClass>().HasData(new { Id = 1, Name = "Jack", LinkToNOFClassId = 1 });

            
        }
    }

    public class EFCoreObjectDbContext : EFCoreTestDbContext {
        public EFCoreObjectDbContext() : base(Constants.CsLegacy) { }
        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();
    }
}