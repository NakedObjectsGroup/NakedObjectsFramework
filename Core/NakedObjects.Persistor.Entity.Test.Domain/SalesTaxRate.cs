// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using NakedObjects;

namespace AdventureWorksModel {
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable InconsistentNaming

    public partial class SalesTaxRate {
        #region Primitive Properties

        #region SalesTaxRateID (Int32)

        [MemberOrder(100)]
        public virtual int SalesTaxRateID { get; set; }

        #endregion

        #region TaxType (Byte)

        [MemberOrder(110)]
        public virtual byte TaxType { get; set; }

        #endregion

        #region TaxRate (Decimal)

        [MemberOrder(120)]
        public virtual decimal TaxRate { get; set; }

        #endregion

        #region Name (String)

        [MemberOrder(130), StringLength(50)]
        public virtual string Name { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(140)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(150), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region StateProvince (StateProvince)

        [MemberOrder(160)]
        public virtual StateProvince StateProvince { get; set; }

        #endregion

        #endregion
    }
}