﻿using AW.Types;
using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Functions
{
    [Named("Addresses")]
    public static class Address_MenuFunctions
    {

        public static IQueryable<Address> RecentAddresses(IContext context) =>
            context.Instances<Address>().OrderByDescending(a => a.ModifiedDate);

        public static (Address, IContext) CreateNewAddress(
            AddressType type,
            string line1,
            string line2,
            string city,
            string postCode,
            [Named("State / Province")] StateProvince sp,
            IContext context)
        {
            var a = new Address() with { AddressLine1 = line1, AddressLine2 = line2, City = city, PostalCode = postCode, StateProvince = sp, ModifiedDate = context.Now(), rowguid = context.NewGuid() };
            return context.SaveAndDisplay(a);
        }

    }
}