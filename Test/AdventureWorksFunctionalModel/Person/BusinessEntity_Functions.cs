using NakedFunctions;
using AW.Types;
using System;

namespace AW.Functions
{

    public static class BusinessEntityFunctions
    {

        //TODO: Need auto-complete for StateProvince
        public static (Address, IContext) CreateNewAddress(
            this BusinessEntity be, 
            AddressType type,
            string line1, 
            string line2, 
            string city, 
            string postCode, 
            StateProvince stateProvince, 
            IContext context)
        {
            var a = new Address() with {AddressLine1 = line1, AddressLine2 = line2, City = city, PostalCode = postCode, StateProvince = stateProvince, ModifiedDate = context.Now(), rowguid = context.NewGuid()  };  //TODO add all fields
            var bea = new BusinessEntityAddress() with { BusinessEntity = be, Address = a, AddressType = type, ModifiedDate = context.Now(), rowguid = context.NewGuid() }; //TODO add all fields
            return (a, context.WithPendingSave(a, bea));
        }


        //TODO: Add params for all mandatory fields of person.
        internal static (Person, IContext) CreateNewContact(
            this IBusinessEntity forEntity, 
            string firstName, 
            string LastName, 
            [Password] string initialPassword,
            IContext context)
        {
            throw new NotImplementedException();
            //var p = Person_MenuFunctions.CreateNewPerson(initialPassword, context);
            //TODO: Create new BusinessEntityContact. Ssve both, return the Person
            //Don't forget ModifiedDate & rowguid on either/both if needed.

        }
    }
}
