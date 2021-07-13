using NakedFunctions;
using AW.Types;
using System.Collections.Generic;
using System.Linq;

namespace AW.Functions
{

    public static class BusinessEntityFunctions
    {
        public static (Address, IContext) CreateNewAddress(
            this BusinessEntity be, 
            AddressType type,
            string line1, 
            string line2, 
            string city, 
            string postCode, 
            [Named("Country / Region")] CountryRegion countryRegion,
            [Named("State / Province")] StateProvince sp, 
            IContext context)
        {
            var a = new Address() with {AddressLine1 = line1, AddressLine2 = line2, City = city, PostalCode = postCode, StateProvince = sp, ModifiedDate = context.Now(), rowguid = context.NewGuid()  }; 
            var bea = new BusinessEntityAddress() with { 
                BusinessEntityID = be.BusinessEntityID, 
                Address = a, 
                AddressTypeID = type.AddressTypeID, 
                ModifiedDate = context.Now(), 
                rowguid = context.NewGuid() };
            return (a, context.WithNew(a).WithNew(bea));
        }

        public static IList<StateProvince> Choices7CreateNewAddress(this Address be, CountryRegion countryRegion, IContext context) =>
            countryRegion is null ? new List<StateProvince>() : Address_Functions.StateProvincesForCountry(countryRegion, context).ToList();


        public static (BusinessEntity, IContext) CreateNewEmailAddress(
            this BusinessEntity be,
            [MaxLength(50)] string emailAddress, //TODO: Add RegEx for emailAddress
            IContext context)
        {
            var e = new EmailAddress {
                BusinessEntityID = be.BusinessEntityID,
                EmailAddress1 = emailAddress,
                rowguid = context.NewGuid(),
                ModifiedDate = context.Now(),
            };
            return (be, context.WithNew(e));
        }

    }
}
