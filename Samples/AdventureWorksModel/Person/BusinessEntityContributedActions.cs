using NakedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel {
    /// <summary>
    /// Defines actions contributed to a business entity for managing associated addresses and contacts
    /// </summary>
    public class BusinessEntityContributedActions {

        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        #endregion

        private const string addresses = "Addresses";

        public Address CreateNewAddress( [ContributedAction(addresses)] IBusinessEntity forEntity) {
            var _Address = Container.NewTransientInstance<Address>();
            _Address.AddressFor = forEntity;
            return _Address;
        }
       
        [TableView(false, "AddressType", "Address")]
        public IList<BusinessEntityAddress> ListAddresses([ContributedAction(addresses)] IBusinessEntity forEntity) {
            var entityId = forEntity.BusinessEntityID;
            var q = from l in Container.Instances<BusinessEntityAddress>()
                    where l.BusinessEntityID == entityId
                    select l;
            return q.ToList();
        }

        private const string contacts = "Contacts";

        public Person CreateNewContact([ContributedAction(contacts)] IBusinessEntityWithContacts forEntity) {
            var person = Container.NewTransientInstance<Person>();
            person.ForEntity = forEntity;
            return person;
        }

        [TableView(false, "ContactType", "Person")]
        public IList<BusinessEntityContact> ListContacts([ContributedAction(contacts)] IBusinessEntityWithContacts forEntity) {
            var entityId = forEntity.BusinessEntityID;
            var q = from l in Container.Instances<BusinessEntityContact>()
                    where l.BusinessEntityID == entityId
                    select l;
            return q.ToList();
        }

    }
}
