// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

using System;
using Microsoft.EntityFrameworkCore;

namespace TestData {
    public class EFCoreTestDataContext : DbContext {
        private readonly string cs;

        public EFCoreTestDataContext(string cs) => this.cs = cs;

        public DbSet<Person> People { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderFail> OrderFails { get; set; }

        public void Delete() => Database.EnsureDeleted();

        public void Create() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(cs);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            int NewPerson(int id, string name, int productId, bool address = false, int? relatedTo = null) {
                string l1 = address ? "L1" : null;
                string l2 = address ? "L2" : null;

                modelBuilder.Entity<Person>().HasData(new { PersonId = id, Name = name, FavouriteProductId = productId, PersonId1 = relatedTo, Address_Line1 = l1, Address_Line2 = l2});
                
                return id;
            }

            int NewProduct(int id, string name) {
                modelBuilder.Entity<Product>().HasData(new Product { Id = id, Name = name, ModifiedDate = DateTime.Now.ToBinary().ToString() });
                return id;
            }

            int NewPet(int id, string name, int personId) {
                modelBuilder.Entity<Pet>().HasData(new { PetId = id, Name = name, OwnerId = personId });
                return id;
            }

            var product1 = NewProduct(1, "ProductOne");
            var product2 = NewProduct(2, "ProductTwo");
            var product3 = NewProduct(3, "ProductThree");
            var product4 = NewProduct(4, "ProductFour");
            var person1 = NewPerson (1, "PersonOne", product1, true);
            var person2 = NewPerson (2, "PersonTwo", product1, true, person1);
            var person3 = NewPerson (3, "PersonThree", product2, true, person1);
            var person4 = NewPerson (4, "PersonFour", product2, true);
            var person5 = NewPerson (5, "PersonFive", product2);
            var person6 = NewPerson (6, "PersonSix", product2);
            var person7 = NewPerson (7, "PersonSeven", product2, false, person1);
            var person8 = NewPerson (8, "PersonEight", product2, false, person1);
            var person9 = NewPerson (9, "PersonNine", product2, false, person6);
            var person10 = NewPerson (10, "PersonTen", product2, false, person7);
            var person11 = NewPerson (11, "PersonEleven", product2, false, person8);
            var person12 = NewPerson (12, "PersonTwelve", product4);
            var person13 = NewPerson (13, "PersonThirteen", product4);
            var person14 = NewPerson (14, "PersonFourteen", product4);
            var person15 = NewPerson (15, "PersonFifteen", product4);
            var person16 = NewPerson (16, "PersonSixteen", product4);
            var person17 = NewPerson (17, "PersonSeventeen", product4);
            var person18 = NewPerson (18, "PersonEighteen", product4);
            var person19 = NewPerson (19, "PersonNineteen", product4);
            var person20 = NewPerson (20, "PersonTwenty", product4);
            var person21 = NewPerson (21, "PersonTwentyOne", product4);
            var person22 = NewPerson (22, "PersonTwentyTwo", product4);
            var pet1 = NewPet(1, "PetOne", person1);
            //let products = [| product1; product2 |]
            //let persons = [| person1; person2; person3; person4; person5; person6; person7; person8; person9; person10; person11 |]
            //person1.Relatives.Add(person2)
            //person1.Relatives.Add(person3)
            //person1.Relatives.Add(person7)
            //person1.Relatives.Add(person8)
            //person6.Relatives.Add(person9)
            //person7.Relatives.Add(person10)
            //person8.Relatives.Add(person11)
            //person1.Address.Line1 < -"L1"
            //person1.Address.Line2 < -"L2"
            //person2.Address.Line1 < -"L1"
            //person2.Address.Line2 < -"L2"
            //person3.Address.Line1 < -"L1"
            //person3.Address.Line2 < -"L2"
            //person4.Address.Line1 < -"L1"
            //person4.Address.Line2 < -"L2"
            //products |> Seq.iter(fun x->x.ResetEvents())
            //persons |> Seq.iter(fun x->x.ResetEvents())
            //persons |> Seq.iter(fun x->x.Address.ResetEvents())



        }
    }
}