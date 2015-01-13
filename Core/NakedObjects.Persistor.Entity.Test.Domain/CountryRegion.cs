// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart

    public partial class CountryRegion {
        #region Primitive Properties

        #region CountryRegionCode (String)

        [MemberOrder(100), StringLength(3)]
        public virtual string CountryRegionCode { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region CountryRegionCurrencies (Collection of CountryRegionCurrency)

        private ICollection<CountryRegionCurrency> _countryRegionCurrencies = new List<CountryRegionCurrency>();

        [MemberOrder(130), Disabled]
        public virtual ICollection<CountryRegionCurrency> CountryRegionCurrencies {
            get { return _countryRegionCurrencies; }
            set { _countryRegionCurrencies = value; }
        }

        #endregion

        #region StateProvinces (Collection of StateProvince)

        private ICollection<StateProvince> _stateProvinces = new List<StateProvince>();

        [MemberOrder(140), Disabled]
        public virtual ICollection<StateProvince> StateProvinces {
            get { return _stateProvinces; }
            set { _stateProvinces = value; }
        }

        #endregion

        #endregion
    }
}