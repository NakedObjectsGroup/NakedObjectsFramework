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
using NakedObjects;

namespace AdventureWorksModel {
    [IconName("house.png")]
    [Immutable(WhenTo.OncePersisted)]
    public class Address : AWDomainObject {
        #region Injected Services

        public ContactRepository ContactRepository { set; protected get; }

        #endregion

        #region Title

        public override string ToString() {
            var t = Container.NewTitleBuilder();
            t.Append(AddressLine1).Append("...");
            return t.ToString();
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

        [NakedObjectsIgnore]
        public virtual int StateProvinceID { get; set; }

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

        public IList<CountryRegion> ChoicesCountryRegion() {
            return ContactRepository.ValidCountries();
        }

        #endregion
    }
}