// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class CurrencyRate {
        #region Primitive Properties

        #region CurrencyRateID (Int32)

        [MemberOrder(100)]
        public virtual int CurrencyRateID { get; set; }

        #endregion

        #region CurrencyRateDate (DateTime)

        [MemberOrder(110), Mask("d")]
        public virtual DateTime CurrencyRateDate { get; set; }

        #endregion

        #region AverageRate (Decimal)

        [MemberOrder(120)]
        public virtual decimal AverageRate { get; set; }

        #endregion

        #region EndOfDayRate (Decimal)

        [MemberOrder(130)]
        public virtual decimal EndOfDayRate { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(140), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Currency (Currency)

        [MemberOrder(150)]
        public virtual Currency Currency { get; set; }

        #endregion

        #region Currency1 (Currency)

        [MemberOrder(160)]
        public virtual Currency Currency1 { get; set; }

        #endregion

        #region SalesOrderHeaders (Collection of SalesOrderHeader)

        private ICollection<SalesOrderHeader> _salesOrderHeaders = new List<SalesOrderHeader>();

        [MemberOrder(170), Disabled]
        public virtual ICollection<SalesOrderHeader> SalesOrderHeaders {
            get { return _salesOrderHeaders; }
            set { _salesOrderHeaders = value; }
        }

        #endregion

        #endregion
    }
}