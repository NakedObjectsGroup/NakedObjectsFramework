// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace TestData {
    [ValidateProgrammaticUpdates]
    public class Person : TestHelper {
        private Address address = new Address();
        private string name;
        private ICollection<Person> relatives = new List<Person>();
        private bool updateInPersisting;
        private bool persistInUpdated;

        [Key]
        public virtual int PersonId { get; set; }

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

// ReSharper disable once ParameterHidesMember
        public string Validate(string name, Product favouriteProduct) {
            if (name == "fail") {
                return name;
            }
            return null;
        }

        public IQueryable<Person> FindRelativesByName(IQueryable<Person> persons, string newName) {
            return (from r in persons
                where r.Name == newName
                select r).AsQueryable();
        }

        [Executed(Where.Remotely)]
        public string DisableFindRelativesByName(IQueryable<Person> persons, string newName) {
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

        public void UpdateInPersisting() {
            updateInPersisting = true;
        }

        public void PersistInUpdated() {
            persistInUpdated = true;
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

        public override void Updated() {
            base.Updated();

            if (persistInUpdated) {
                // 
                var product = Container.NewTransientInstance<Product>();
                product.Name = "ProductTest";
                Container.Persist(ref product);
            }
        }
    }
}