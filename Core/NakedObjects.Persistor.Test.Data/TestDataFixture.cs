// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Util;
using TestData;

namespace NakedObjects.Persistor.TestData {
    public class TestDataFixture  {

        public IDomainObjectContainer Container { protected get; set; }


        public  void Install() {
            Product product1 = NewProduct(1, "ProductOne");
            Product product2 = NewProduct(2, "ProductTwo");
            Product product3 = NewProduct(3, "ProductThree");
            Product product4 = NewProduct(4, "ProductFour");

            Person person1 = NewPerson(1, "PersonOne", product1);
            Person person2 = NewPerson(2, "PersonTwo", product1);
            Person person3 = NewPerson(3, "PersonThree", product2);
            Person person4 = NewPerson(4, "PersonFour", product2);
            Person person5 = NewPerson(5, "PersonFive", product2);
            Person person6 = NewPerson(6, "PersonSix", product2);
            Person person7 = NewPerson(7, "PersonSeven", product2);
            Person person8 = NewPerson(8, "PersonEight", product2);
            Person person9 = NewPerson(9, "PersonNine", product2);
            Person person10 = NewPerson(10, "PersonTen", product2);
            Person person11 = NewPerson(11, "PersonEleven", product2);
            Person person12 = NewPerson(12, "PersonTwelve", product4);
            Person person13 = NewPerson(13, "PersonThirteen", product4);
            Person person14 = NewPerson(14, "PersonFourteen", product4);
            Person person15 = NewPerson(15, "PersonFifteen", product4);
            Person person16 = NewPerson(16, "PersonSixteen", product4);
            Person person17 = NewPerson(17, "PersonSeventeen", product4);
            Person person18 = NewPerson(18, "PersonEighteen", product4);
            Person person19 = NewPerson(19, "PersonNineteen", product4);
            Person person20 = NewPerson(20, "PersonTwenty", product4);
            Person person21 = NewPerson(21, "PersonTwentyOne", product4);
            Person person22 = NewPerson(22, "PersonTwentyTwo", product4);

            Pet pet1 = NewPet(1, "PetOne", person1);

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

        private Order NewOrder(int id, string name) {
            var order = Container.NewTransientInstance<Order>();
            order.OrderId = id;
            order.Name = name;

            Container.Persist(ref order);
            return order;
        }
    }
}