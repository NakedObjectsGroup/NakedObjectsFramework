﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace TestCodeOnly;
public class BlankTriggerAddingConvention : IModelFinalizingConvention {
    public virtual void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context) {
        foreach (var entityType in modelBuilder.Metadata.GetEntityTypes()) {
            var table = StoreObjectIdentifier.Create(entityType, StoreObjectType.Table);
            if (table != null
                && entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(table.Value) == null)) {
                entityType.Builder.HasTrigger(table.Value.Name + "_Trigger");
            }

            foreach (var fragment in entityType.GetMappingFragments(StoreObjectType.Table)) {
                if (entityType.GetDeclaredTriggers().All(t => t.GetDatabaseName(fragment.StoreObject) == null)) {
                    entityType.Builder.HasTrigger(fragment.StoreObject.Name + "_Trigger");
                }
            }
        }
    }
}
public class EFCoreCodeFirstContext : DbContext {
    private readonly string cs;

    public EFCoreCodeFirstContext(string cs) => this.cs = cs;

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CountryCode> CountryCodes { get; set; }
    public DbSet<DomesticAddress> DomesticAddresses { get; set; }
    public DbSet<InternationalAddress> InternationalAddresses { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Product> Products { get; set; }

    public DbSet<Squad> Squads { get; set; }

    public DbSet<System> Systems { get; set; }

    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
        //optionsBuilder.EnableDetailedErrors();
        //optionsBuilder.EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(m => Console.WriteLine(m), LogLevel.Trace);
    }

    //protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
    //    configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<CountryCode>().HasKey(cc => new { cc.Code, cc.ISOCode });

        var food = new Category { ID = 1, Name = "Food" };
        modelBuilder.Entity<Category>().HasData(food);

        modelBuilder.Entity<CountryCode>().HasData(new CountryCode { Code = "USA", ISOCode = 100, Name = "USA" });
        modelBuilder.Entity<CountryCode>().HasData(new CountryCode { Code = "UK", ISOCode = 101, Name = "Great Britain" });
        modelBuilder.Entity<CountryCode>().HasData(new CountryCode { Code = "FR", ISOCode = 102, Name = "France" });

        modelBuilder.Entity<Product>().HasData(new { ID = 1, Name = "Bovril", OwningcategoryID = 1 });
        modelBuilder.Entity<Product>().HasData(new { ID = 2, Name = "Marmite", OwningcategoryID = 1 });
        modelBuilder.Entity<Product>().HasData(new { ID = 3, Name = "Vegemite", OwningcategoryID = 1 });

        modelBuilder.Entity<DomesticAddress>().HasData(new DomesticAddress { ID = 1, Lines = "22 Westleigh Drive", Postcode = "RG4 9LB" });
        modelBuilder.Entity<DomesticAddress>().HasData(new DomesticAddress { ID = 2, Lines = "BNR Park, Concorde Road", Postcode = "SL6 4AG" });
        modelBuilder.Entity<InternationalAddress>().HasData(new { ID = 3, Lines = "1 Madison Avenue, New York", CountryCode = "USA", CountryISOCode = 100 });

        modelBuilder.Entity<Person>().HasData(new { ID = 1, Name = "Ted", FavouriteID = 1, AddressID = 1 });
        modelBuilder.Entity<Person>().HasData(new { ID = 2, Name = "Bob", FavouriteID = 2, AddressID = 2 });
        modelBuilder.Entity<Person>().HasData(new { ID = 3, Name = "Jane", FavouriteID = 3, AddressID = 3 });

        modelBuilder.Entity<Squad>().HasData(new Squad { Id = 1, Name = "squad1" });
        modelBuilder.Entity<System>().HasData(new System { Id = 1, Name = "system1" });

        modelBuilder.Entity<System>(entity => {
            entity.HasMany(d => d.Squads)
                  .WithMany(p => p.Systems)
                  .UsingEntity<Dictionary<string, object>>(
                      "SystemSquadAssignment",
                      l => l.HasOne<Squad>().WithMany().HasForeignKey("SquadId").HasConstraintName("system_squad_assignment_fk_squad_id"),
                      r => r.HasOne<System>().WithMany().HasForeignKey("SystemId").HasConstraintName("system_squad_assignment_fk_system_id"),
                      j => {
                          j.HasKey("SystemId", "SquadId").HasName("system_squad_assignment_pk");

                          j.ToTable("system_squad_assignment", "tracking").ToTable(t => t.HasComment("Associates a system to it's assigned squads"));

                          j.IndexerProperty<int>("SystemId").HasColumnName("system_id");

                          j.IndexerProperty<int>("SquadId").HasColumnName("squad_id");
                      });
        });
    }
}