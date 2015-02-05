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
    // ReSharper disable InconsistentNaming

    public partial class Currency {
        #region Primitive Properties

        #region CurrencyCode (String)

        [MemberOrder(100), StringLength(3)]
        public virtual string CurrencyCode { get; set; }

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

        #region CurrencyRates (Collection of CurrencyRate)

        private ICollection<CurrencyRate> _currencyRates = new List<CurrencyRate>();

        [MemberOrder(140), Disabled]
        public virtual ICollection<CurrencyRate> CurrencyRates {
            get { return _currencyRates; }
            set { _currencyRates = value; }
        }

        #endregion

        #region CurrencyRates1 (Collection of CurrencyRate)

        private ICollection<CurrencyRate> _currencyRates1 = new List<CurrencyRate>();

        [MemberOrder(150), Disabled]
        public virtual ICollection<CurrencyRate> CurrencyRates1 {
            get { return _currencyRates1; }
            set { _currencyRates1 = value; }
        }

        #endregion

        #endregion
    }
}