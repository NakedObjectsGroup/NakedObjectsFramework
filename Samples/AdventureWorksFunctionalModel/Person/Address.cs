// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;
using NakedObjects;


namespace AdventureWorksModel {
    [IconName("house.png")]
    [Immutable(WhenTo.OncePersisted)]
    public class Address : IHasRowGuid, IHasModifiedDate
    {
        public Address(
            int addressID,
            string addressLine1,
            string addressLine2,
            string city,
            string postalCode,
            int stateProvinceID,
            StateProvince stateProvince,
            int countryRegionID,
            CountryRegion countryRegion,
            //int addressTypeID,
            AddressType addressType,
            int addressForID,
            BusinessEntity addressFor,
            Guid rowguid,
            DateTime modifiedDate)
        {
            AddressID = addressID;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            PostalCode = postalCode;
            StateProvinceID = stateProvinceID;
            StateProvince = stateProvince;
            //CountryRegionID = countryRegionID;
            CountryRegion = countryRegion;
            //AddressTypeID = addressTypeID;
            AddressType = addressType;
            //AddressForID = addressForID;
            AddressFor = addressFor;
            this.rowguid = rowguid;
            ModifiedDate = modifiedDate;
        }

        public Address()
        {

        }

        [Disabled]
        public virtual int AddressID { get; set; }

        [MemberOrder(11)]
        [StringLength(60)]
        public virtual string AddressLine1 { get; set; }

        [MemberOrder(12)]
        [Optionally]
        [StringLength(60)]
        public virtual string AddressLine2 { get; set; }

        [MemberOrder(13)]
        [StringLength(30)]
        public virtual string City { get; set; }

        [MemberOrder(14)]
        [StringLength(15)]
        public virtual string PostalCode { get; set; }

        [NakedObjectsIgnore]
        public virtual int StateProvinceID { get; set; }

        [MemberOrder(15)]
        public virtual StateProvince StateProvince { get; set; }

        //[NakedObjectsIgnore]
        //public virtual int CountryRegionID { get; set; }

        [Disabled(WhenTo.OncePersisted)]
        [NotPersisted]
        [Optionally]
        [MemberOrder(16)]
        public virtual CountryRegion CountryRegion { get; set; }

        //[NakedObjectsIgnore]
        //public virtual int AddressTypeID { get; set; }

        [Hidden(WhenTo.OncePersisted)]
        [NotPersisted]
        [MemberOrder(10)]
        public virtual AddressType AddressType { get; set; }

        //[NakedObjectsIgnore]
        //public virtual int AddressForID { get; set; }

        [NotPersisted]
        [Disabled]
        public virtual BusinessEntity AddressFor { get; set; }


        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }


        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

    }

    public static class AddressFunctions
    {
        #region LifeCycle methods
        //Note that the Persisting & Updating methods returns a replacement object that needs to be 
        //swapped for the origina

        public static Address Updating(Address a, [Injected] DateTime now)
        {
            return LifeCycleFunctions.UpdateModified(a, now);

        }

        public static Address Persisting(Address a, [Injected] Guid guid, [Injected] DateTime now )
        {
            return Updating(a, now).With(x => x.rowguid,guid); 
        }

        //Any object or list returned by Persisted (or Updated), is not for display but to be persisted/updated
        //themselves (equivalent to second Tuple value returned from an Action).
        public static BusinessEntityAddress Persisted(Address a, [Injected] Guid guid, [Injected] DateTime now)
        {
            return
                new BusinessEntityAddress(
                //a.AddressForID,
                a.AddressFor,
               //a.AddressTypeID,
                a.AddressType,
                a.AddressID,
                a,
                guid,
                now);
        }
        #endregion

        public static string Title(this Address a)
        {
            return a.CreateTitle($"{a.AddressLine1}...");
        }

        //TODO: Validate and Choices methods were both commented-out in original code, and
        //there is redundancy between them.  Included here (temporarily) for example purposes.
        public static string Validate(Address a, CountryRegion countryRegion, StateProvince stateProvince, [Injected] IQueryable<StateProvince> allProvinces)
        {
            IList<StateProvince> valid = StateProvincesForCountry(countryRegion, allProvinces);

            if (valid.Contains(stateProvince))
            {
                return null;
            }

            return "Invalid region";
        }
        

        //TODO: Although the injected first param is not used here, it is still needed in order to match up this
        //Choices function with the Address type. 
        //TODO: Is Executed relevant any more? Or is it a hangover from early thick-client NOF ?
        [Executed(Where.Remotely)]
        public static IList<StateProvince> ChoicesStateProvince(Address a, CountryRegion countryRegion, [Injected] IQueryable<StateProvince> allProvincences)
        {
            return countryRegion != null ? StateProvincesForCountry(countryRegion, allProvincences) : new List<StateProvince>();
        }

        private static IList<StateProvince> StateProvincesForCountry(CountryRegion country, IQueryable<StateProvince> provinces)
        {
            return provinces.Where(p => p.CountryRegion.CountryRegionCode == country.CountryRegionCode).OrderBy(p => p.Name).ToList();
        }
    }
}