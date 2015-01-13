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

    public partial class CreditCard {
        #region Primitive Properties

        #region CreditCardID (Int32)

        [MemberOrder(100)]
        public virtual int CreditCardID { get; set; }

        #endregion

        #region CardType (String)

        [MemberOrder(110), StringLength(50)]
        public virtual string CardType { get; set; }

        #endregion

        #region CardNumber (String)

        [MemberOrder(120), StringLength(25)]
        public virtual string CardNumber { get; set; }

        #endregion

        #region ExpMonth (Byte)

        [MemberOrder(130)]
        public virtual byte ExpMonth { get; set; }

        #endregion

        #region ExpYear (Int16)

        [MemberOrder(140)]
        public virtual short ExpYear { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(150), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region ContactCreditCards (Collection of ContactCreditCard)

        private ICollection<ContactCreditCard> _contactCreditCards = new List<ContactCreditCard>();

        [MemberOrder(160), Disabled]
        public virtual ICollection<ContactCreditCard> ContactCreditCards {
            get { return _contactCreditCards; }
            set { _contactCreditCards = value; }
        }

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