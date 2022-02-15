// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Constraints;

namespace NOF2.Rest.Test.Data;

public static class Constants {
    public static string AppveyorServer => @"(local)\SQL2017";
    public static string LocalServer => @"(localdb)\MSSQLLocalDB;";

#if APPVEYOR
        public static string Server => AppveyorServer;
#else
    public static string Server => LocalServer;
#endif

    public static readonly string CsNOF2 = @$"Data Source={Server};Initial Catalog={"NOF2Tests"};Integrated Security=True;";
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
    public DbSet<NOF2ClassWithInterface> NOF2ClassWithInterfaces { get; set; }
    public DbSet<ClassWithNOF2Interface> ClassWithNOF2Interfaces { get; set; }
    public DbSet<ClassWithMenu> ClassWithMenus { get; set; }
    public DbSet<ClassWithDate> ClassWithDates { get; set; }
    public DbSet<ClassWithDate> ClassWithTimeStamps { get; set; }
    public DbSet<ClassWithWholeNumber> ClassWithWholeNumbers { get; set; }
    public DbSet<ClassWithLogical> ClassWithLogicals { get; set; }
    public DbSet<ClassWithMoney> ClassWithMoneys { get; set; }
    public DbSet<ClassWithOrderedProperties> ClassesWithOrderedProperties { get; set; }
    public DbSet<ClassWithOrderedActions> ClassesWithOrderedActions { get; set; }
    public DbSet<ClassWithReferenceProperty> ClassWithReferenceProperties { get; set; }
    public DbSet<ClassWithBounded> ClassWithBoundeds { get; set; }
    public DbSet<ClassToPersistWithAbout> ClassesToPersistWithAbout { get; set; }

    public DbSet<ClassToPersist> ClassesToPersist { get; set; }
    public DbSet<ClassWithAnnotations> ClassesWithAnnotations { get; set; }

    public DbSet<ClassWithInvalidNames> ClassWithInvalidNames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
    }

    private static void MapClassWithTextString(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithTextString>().Ignore(t => t.Name);
        modelBuilder.Entity<ClassWithTextString>().Property("name").HasColumnName("Name");
    }

    private static void MapClassToPersistWithAbout(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassToPersistWithAbout>().Ignore(t => t.Name).Ignore(t => t.Container);
        modelBuilder.Entity<ClassToPersistWithAbout>().Property("name").HasColumnName("Name");
    }

    private static void MapClassToPersist(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClassToPersist>().Ignore(t => t.Name).Ignore(t => t.Container);
        modelBuilder.Entity<ClassToPersist>().Property("name").HasColumnName("Name");
    }

    private static void MapClassWithFieldAbout(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithFieldAbout>().Ignore(t => t.Name);
        modelBuilder.Entity<ClassWithTextString>().Property("name").HasColumnName("Name");
    }

    private static void MapClassWithReferenceProperty(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithReferenceProperty>().Ignore(t => t.Container);
    }

    private static void MapClassWithBounded(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithBounded>().Ignore(t => t.Name);
        modelBuilder.Entity<ClassWithBounded>().Ignore(t => t.ChoicesProperty);
        modelBuilder.Entity<ClassWithBounded>().Property("name").HasColumnName("Name");
    }

    private static void MapClassWithDate(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithDate>().Ignore(t => t.Date);
        modelBuilder.Entity<ClassWithDate>().Property("date").HasColumnName("Date");
        modelBuilder.Entity<ClassWithDate>().Ignore(t => t.DateNullable);
        modelBuilder.Entity<ClassWithDate>().Property("date1").HasColumnName("Date1");
    }

    private static void MapClassWithTimeStamp(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithTimeStamp>().Ignore(t => t.TimeStamp);
        modelBuilder.Entity<ClassWithTimeStamp>().Property("date").HasColumnName("Date");
    }

    private static void MapClassWithWholeNumber(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithWholeNumber>().Ignore(t => t.WholeNumber);
        modelBuilder.Entity<ClassWithWholeNumber>().Property("number").HasColumnName("Number");
    }

    private static void MapClassWithLogical(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithLogical>().Ignore(t => t.Logical);
        modelBuilder.Entity<ClassWithLogical>().Property("boolean").HasColumnName("Boolean");
    }

    private static void MapClassWithMoney(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithMoney>().Ignore(t => t.Money);
        modelBuilder.Entity<ClassWithMoney>().Property("amount").HasColumnName("Amount");
    }

    private static void MapClassWithInternalCollection(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithInternalCollection>().Ignore(t => t.TestCollection);

        modelBuilder.Entity<ClassWithInternalCollection>()
                    .HasMany(c => c._TestCollection);
    }

    private static void MapClassWithOrderedProperties(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithOrderedProperties>().Ignore(t => t.Name1);
        modelBuilder.Entity<ClassWithOrderedProperties>().Ignore(t => t.Name2);
        modelBuilder.Entity<ClassWithOrderedProperties>().Ignore(t => t.Name3);
        modelBuilder.Entity<ClassWithOrderedProperties>().Property("name1").HasColumnName("Name1");
        modelBuilder.Entity<ClassWithOrderedProperties>().Property("name2").HasColumnName("Name2");
        modelBuilder.Entity<ClassWithOrderedProperties>().Property("name3").HasColumnName("Name3");
    }

    private static void MapClassWithAnnotations(ModelBuilder modelBuilder) {
        modelBuilder.Entity<ClassWithAnnotations>().Ignore(t => t.Name).Ignore(t => t.TestTableView);
        modelBuilder.Entity<ClassWithAnnotations>().Property("name").HasColumnName("Name");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        MapClassWithTextString(modelBuilder);
        MapClassWithDate(modelBuilder);
        MapClassWithTimeStamp(modelBuilder);
        MapClassWithInternalCollection(modelBuilder);
        MapClassWithWholeNumber(modelBuilder);
        MapClassWithLogical(modelBuilder);
        MapClassWithMoney(modelBuilder);
        MapClassWithBounded(modelBuilder);
        MapClassWithReferenceProperty(modelBuilder);
        MapClassWithFieldAbout(modelBuilder);
        MapClassToPersistWithAbout(modelBuilder);
        MapClassToPersist(modelBuilder);
        MapClassWithAnnotations(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder) {
        var fred = new { Id = 1, name = "Fred", ClassWithInternalCollectionId = 1, ClassWithStringId = 2 };
        var bill = new { Id = 2, name = "Bill", ClassWithStringId = 2 };
        var jane = new { Id = 1, date = new DateTime(2021, 11, 1), date1 = new DateTime(2021, 11, 1) };

        modelBuilder.Entity<ClassWithTimeStamp>().HasData(jane);

        modelBuilder.Entity<ClassWithInternalCollection>().HasData(new ClassWithInternalCollection { Id = 1 });
        modelBuilder.Entity<ClassWithInternalCollection>().HasData(new ClassWithInternalCollection { Id = 2 });

        modelBuilder.Entity<ClassWithNOFInternalCollection>().HasData(new { Id = 1 });

        modelBuilder.Entity<ClassWithString>().HasData(new { Id = 2, Name = "Ted", ClassWithNOFInternalCollectionId = 1 });

        modelBuilder.Entity<ClassWithTextString>().HasData(fred);
        modelBuilder.Entity<ClassWithTextString>().HasData(bill);
        modelBuilder.Entity<ClassWithTextString>().HasData(new {Id = 3, name = "Tom"});

        modelBuilder.Entity<ClassWithActionAbout>().HasData(new { Id = 1 });
        modelBuilder.Entity<ClassWithFieldAbout>().HasData(new { Id = 1 });
        modelBuilder.Entity<ClassWithMenu>().HasData(new { Id = 1 });
        modelBuilder.Entity<ClassWithDate>().HasData(jane);

        modelBuilder.Entity<NOF2ClassWithInterface>().HasData(new { Id = 10 });
        modelBuilder.Entity<ClassWithNOF2Interface>().HasData(new { Id = 10 });

        modelBuilder.Entity<ClassWithString>().HasData(new { Id = 1, Name = "Jill", LinkToNOF2ClassId = 1, ClassWithNOFInternalCollectionId = 1 });

        modelBuilder.Entity<ClassWithLinkToNOFClass>().HasData(new { Id = 1, Name = "Jack", LinkToNOFClassId = 1 });

        modelBuilder.Entity<ClassWithWholeNumber>().HasData(new ClassWithWholeNumber { Id = 1, number = 10 });
        modelBuilder.Entity<ClassWithLogical>().HasData(new ClassWithLogical { Id = 1, boolean = true });
        modelBuilder.Entity<ClassWithMoney>().HasData(new ClassWithMoney { Id = 1, amount = 10.00M });
        modelBuilder.Entity<ClassWithReferenceProperty>().HasData(new { Id = 1, ReferencePropertyId = 1 });
        modelBuilder.Entity<ClassWithOrderedProperties>().HasData(new { Id = 1 });
        modelBuilder.Entity<ClassWithOrderedActions>().HasData(new { Id = 1 });

        modelBuilder.Entity<ClassWithBounded>().HasData(new ClassWithBounded { Id = 1, name = "data1" });
        modelBuilder.Entity<ClassWithBounded>().HasData(new ClassWithBounded { Id = 2, name = "data2" });

        modelBuilder.Entity<ClassWithAnnotations>().HasData(new ClassWithAnnotations { Id = 1, name = "data1" });

        modelBuilder.Entity<ClassWithInvalidNames>().HasData(new ClassWithInvalidNames { Id = 1 });
    }
}

public class EFCoreObjectDbContext : EFCoreTestDbContext {
    public EFCoreObjectDbContext() : base(Constants.CsNOF2) { }
    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();
}