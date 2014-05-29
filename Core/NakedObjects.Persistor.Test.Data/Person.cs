// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace TestData {
    [ValidateProgrammaticUpdates]
    public class Person : TestHelper {
        private Address address = new Address();
        private ICollection<Person> relatives = new List<Person>();

        [Key]
        public virtual int PersonId { get; set; }

        private string name;

        [Title]
        [Optionally]
        public virtual string Name {
            get { return name; }
            set { name = value; }
        }

        [Optionally]
        public virtual Product FavouriteProduct { get; set; }

        [Optionally]
        public virtual Pet Pet { get; set; }

        public virtual Address Address {
            get { return address; }
            set { address = value; }
        }

        public virtual ICollection<Person> Relatives {
            get { return relatives; }
            set { relatives = value; }
        }

        public virtual void AddToRelatives(Person person) {
            Relatives.Add(person);
        }

        public virtual void RemoveFromRelatives(Person person) {
            Relatives.Remove(person);
        }

        public virtual void ClearRelatives() {
            Relatives.Clear();
        }

        public void ChangeName(string newName) {
            name = newName;
        }


        public string Validate(string name, Product favouriteProduct) {
            if (name == "fail") {
                return name;
            }
            return null;
        }

        public IQueryable<Person> FindRelativesByName(IQueryable<Person> persons, string name) {
            return (from r in persons
                    where r.Name == name
                    select r).AsQueryable();
        }

        [Executed(Where.Remotely)]
        public string DisableFindRelativesByName(IQueryable<Person> persons, string name) {
            return "disabled";
        }

        public IEnumerable<Person> FindRelativesById(IQueryable<Person> persons, int id) {
            return from r in persons
                   where r.PersonId == id
                   select r;
        }

        [Executed(Where.Remotely)]
        public bool HideFindRelativesById() {
            return true;
        }

        public bool updateInPersisting; 
        public void UpdateInPersisting() {
            updateInPersisting = true; 
        }

        public override void Persisting() {
            base.Persisting();

            if (updateInPersisting) {
                // update  to test updating/updated not called 
                var product = Container.NewTransientInstance<Product>();
                product.Name = "ProductOne";
                FavouriteProduct = product;
            }
        }

    }
}