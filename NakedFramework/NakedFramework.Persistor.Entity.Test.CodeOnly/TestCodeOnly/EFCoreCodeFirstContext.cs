// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

using Microsoft.EntityFrameworkCore;

namespace TestCodeOnly;

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

    public void Delete() => Database.EnsureDeleted();

    public void Create() => Database.EnsureCreated();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(cs);
        optionsBuilder.UseLazyLoadingProxies();
        //optionsBuilder.EnableDetailedErrors();
        //optionsBuilder.EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(m => Console.WriteLine(m), LogLevel.Trace);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<CountryCode>().HasKey(cc => new {cc.Code, cc.ISOCode});

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
        modelBuilder.Entity<InternationalAddress>().HasData(new { ID = 3, Lines = "1 Madison Avenue, New York", CountryCode = "USA", CountryISOCode= 100 });

        modelBuilder.Entity<Person>().HasData(new { ID = 1, Name = "Ted", FavouriteID = 1, AddressID = 1 });
        modelBuilder.Entity<Person>().HasData(new { ID = 2, Name = "Bob", FavouriteID = 2, AddressID = 2 });
        modelBuilder.Entity<Person>().HasData(new { ID = 3, Name = "Jane", FavouriteID = 3, AddressID = 3 });
    }
}