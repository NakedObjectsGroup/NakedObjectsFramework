// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("house.png")]
    [Immutable(WhenTo.OncePersisted)]
    public class Address : AWDomainObject {
        #region Title

        public override string ToString() {
            var t = new TitleBuilder();
            t.Append(AddressLine1).Append("...");
            return t.ToString();
        }
        #endregion

        #region Injected Services

        public ContactRepository ContactRepository { set; protected get; }

        #endregion

        #region Life Cycle Methods

        [Hidden(WhenTo.OncePersisted)]
        [NotPersisted]
        [MemberOrder(10)]
        public virtual AddressType AddressType { get; set; }

        [Hidden]
        [NotPersisted]
        public virtual Customer ForCustomer { get; set; }


        public void Persisted() {
            var ca = Container.NewTransientInstance<CustomerAddress>();
            ca.Address = this;
            ca.AddressType = AddressType;
            ca.Customer = ForCustomer;       
            Container.Persist(ref ca);
        }

        #endregion

        #region Properties
        
        [Hidden]
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

        [MemberOrder(15)]
        public virtual StateProvince StateProvince { get; set; }

        [Executed(Where.Remotely)]
        public IList<StateProvince> ChoicesStateProvince(CountryRegion countryRegion) {
            return countryRegion != null ? StateProvincesForCountry(countryRegion) : new List<StateProvince>();
        }

        #endregion

        #region Row Guid and Modified Date

        #region rowguid

        [Hidden]
        public override Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        public override DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region CountryRegion (derived)


        [Disabled(WhenTo.OncePersisted)]
        [NotPersisted]
        [MemberOrder(16)]
        public virtual CountryRegion CountryRegion { get; set; }

        public IList<CountryRegion> ChoicesCountryRegion()
        {
            return ContactRepository.ValidCountries();
        }

        #endregion

        #region StateProvincesForCountry

        private IList<StateProvince> StateProvincesForCountry(CountryRegion country) {
            var query = from obj in Container.Instances<StateProvince>()
                                                     where obj.CountryRegion.CountryRegionCode == country.CountryRegionCode
                                                     orderby obj.Name
                                                     select obj;

            return query.ToList();
        }

        #endregion

        public string Validate(CountryRegion countryRegion, StateProvince stateProvince) {
            IList<StateProvince> valid = StateProvincesForCountry(countryRegion);

            if (valid.Contains(stateProvince)) {
                return null;
            }

            return "Invalid region";
        }
    }
}