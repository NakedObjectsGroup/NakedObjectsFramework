






using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    public static class Address_Functions {
        internal static IContext UpdateAddress(
            Address original, Address updated, IContext context) =>
            context.WithUpdated(original, new(updated) { ModifiedDate = context.Now() });

        public static IContext EditAddressLines(this Address a,
                                                string line1,
                                                [Optionally] string line2,
                                                string city,
                                                string postalCode,
                                                IContext context) =>
            UpdateAddress(a, new(a) {
                AddressLine1 = line1,
                AddressLine2 = line2,
                City = city,
                PostalCode = postalCode
            }, context);

        public static IContext EditStateProvince(this Address a,
                                                 CountryRegion countryRegion, StateProvince stateProvince, IContext context) =>
            UpdateAddress(a, new(a) { StateProvince = stateProvince }, context);

        public static IList<CountryRegion> Choices1EditStateProvince(this Address a, IContext context) =>
            context.Instances<CountryRegion>().ToArray();

        public static IList<StateProvince> Choices2EditStateProvince(this Address a,
                                                                     CountryRegion countryRegion, IContext context) =>
            StateProvincesForCountry(countryRegion, context);

        internal static StateProvince[] StateProvincesForCountry(this CountryRegion country,
                                                                 IContext context) =>
            context.Instances<StateProvince>().Where(p => p.CountryRegion.CountryRegionCode == country.CountryRegionCode).OrderBy(p => p.Name).ToArray();
    }
}