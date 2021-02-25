// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Core.Util;
using TestData;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Persistor.TestData {
    public class TestDataFixture {
        public IDomainObjectContainer Container { protected get; set; }
        // ReSharper disable UnusedVariable

        public void Install() {
            var product1 = NewProduct(1, "ProductOne");
            var product2 = NewProduct(2, "ProductTwo");
            var product3 = NewProduct(3, "ProductThree");
            var product4 = NewProduct(4, "ProductFour");

            var person1 = NewPerson(1, "PersonOne", product1);
            var person2 = NewPerson(2, "PersonTwo", product1);
            var person3 = NewPerson(3, "PersonThree", product2);
            var person4 = NewPerson(4, "PersonFour", product2);
            var person5 = NewPerson(5, "PersonFive", product2);
            var person6 = NewPerson(6, "PersonSix", product2);
            var person7 = NewPerson(7, "PersonSeven", product2);
            var person8 = NewPerson(8, "PersonEight", product2);
            var person9 = NewPerson(9, "PersonNine", product2);
            var person10 = NewPerson(10, "PersonTen", product2);
            var person11 = NewPerson(11, "PersonEleven", product2);
            var person12 = NewPerson(12, "PersonTwelve", product4);
            var person13 = NewPerson(13, "PersonThirteen", product4);
            var person14 = NewPerson(14, "PersonFourteen", product4);
            var person15 = NewPerson(15, "PersonFifteen", product4);
            var person16 = NewPerson(16, "PersonSixteen", product4);
            var person17 = NewPerson(17, "PersonSeventeen", product4);
            var person18 = NewPerson(18, "PersonEighteen", product4);
            var person19 = NewPerson(19, "PersonNineteen", product4);
            var person20 = NewPerson(20, "PersonTwenty", product4);
            var person21 = NewPerson(21, "PersonTwentyOne", product4);
            var person22 = NewPerson(22, "PersonTwentyTwo", product4);

            var pet1 = NewPet(1, "PetOne", person1);

            var products = new[] {product1, product2};
            var persons = new[] {person1, person2, person3, person4, person5, person6, person7, person8, person9, person10, person11};

            person1.Relatives.Add(person2);
            person1.Relatives.Add(person3);
            person1.Relatives.Add(person7);
            person1.Relatives.Add(person8);

            person6.Relatives.Add(person9);
            person7.Relatives.Add(person10);
            person8.Relatives.Add(person11);

            person1.Address.Line1 = "L1";
            person1.Address.Line2 = "L2";
            person2.Address.Line1 = "L1";
            person2.Address.Line2 = "L2";
            person3.Address.Line1 = "L1";
            person3.Address.Line2 = "L2";
            person4.Address.Line1 = "L1";
            person4.Address.Line2 = "L2";

            products.ForEach(x => x.ResetEvents());
            persons.ForEach(x => x.ResetEvents());
            persons.ForEach(x => x.Address.ResetEvents());
        }

        // ReSharper restore UnusedVariable

        private Person NewPerson(int id, string name, Product product) {
            var person = Container.NewTransientInstance<Person>();
            person.PersonId = id;
            person.Name = name;
            person.FavouriteProduct = product;
            Container.Persist(ref person);
            return person;
        }

        private Product NewProduct(int id, string name) {
            var product = Container.NewTransientInstance<Product>();
            product.Id = id;
            product.Name = name;
            Container.Persist(ref product);
            return product;
        }

        private Pet NewPet(int id, string name, Person person) {
            var pet = Container.NewTransientInstance<Pet>();
            pet.PetId = id;
            pet.Name = name;
            pet.Owner = person;
            Container.Persist(ref pet);
            return pet;
        }
    }
}