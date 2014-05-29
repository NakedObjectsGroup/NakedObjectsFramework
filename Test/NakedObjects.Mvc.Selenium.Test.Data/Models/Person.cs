using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace NakedObjects.Web.UnitTests.Models {
    [NotPersisted]
    public class Person {
        #region Injected Services

        public IDomainObjectContainer Container { set; protected get; }

        #endregion

        #region Life Cycle Methods

        #endregion

        [Key, Hidden]
        public virtual int PersonId { get; set; }

        [Title]
        [MemberOrder(1)]
        public virtual string Name { get; set; }
  
        public virtual string ValidateName (string newName) {
            if (newName.Contains("fail")) {
                return newName;
            }
            return null;
        }

        [Optionally]
        [MemberOrder(2)]
        public virtual Pet FavouritePet { get; set; }

        #region Pets (collection)

        private ICollection<Pet> _Pets = new List<Pet>();

        [MemberOrder(3)]
        public virtual ICollection<Pet> Pets {
            get { return _Pets; }
            set { _Pets = value; }
        }

        public void AddToPets(Pet value) {
            if (!(_Pets.Contains(value))) {
                _Pets.Add(value);
            }
        }

        public void RemoveFromPets(Pet value) {
            if (_Pets.Contains(value)) {
                _Pets.Remove(value);
            }
        }

        public IList<Pet> ChoicesRemoveFromPets(Pet value) {
            return _Pets.ToList();
        }

        #endregion

        #region NewPet (Action)

        [MemberOrder(Sequence = "1")]
        public Pet NewPet(string name) {
            var pet = Container.NewTransientInstance<Pet>();
            pet.Name = name;

            return pet;
        }

        #endregion
    }
}