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

    public partial class SalesOrderHeader {
        #region Primitive Properties

        #region SalesOrderID (Int32)

        [MemberOrder(100)]
        public virtual int SalesOrderID { get; set; }

        #endregion

        #region RevisionNumber (Byte)

        [MemberOrder(110)]
        public virtual byte RevisionNumber { get; set; }

        #endregion

        #region OrderDate (DateTime)

        [MemberOrder(120), Mask("d")]
        public virtual DateTime OrderDate { get; set; }

        #endregion

        #region DueDate (DateTime)

        [MemberOrder(130), Mask("d")]
        public virtual DateTime DueDate { get; set; }

        #endregion

        #region ShipDate (DateTime)

        [MemberOrder(140), Optionally, Mask("d")]
        public virtual DateTime? ShipDate { get; set; }

        #endregion

        #region Status (Byte)

        [MemberOrder(150)]
        public virtual byte Status { get; set; }

        #endregion

        #region OnlineOrderFlag (Boolean)

        [MemberOrder(160)]
        public virtual bool OnlineOrderFlag { get; set; }

        #endregion

        #region SalesOrderNumber (String)

        [MemberOrder(170), StringLength(25)]
        public virtual string SalesOrderNumber { get; set; }

        #endregion

        #region PurchaseOrderNumber (String)

        [MemberOrder(180), Optionally, StringLength(25)]
        public virtual string PurchaseOrderNumber { get; set; }

        #endregion

        #region AccountNumber (String)

        [MemberOrder(190), Optionally, StringLength(15)]
        public virtual string AccountNumber { get; set; }

        #endregion

        #region CreditCardApprovalCode (String)

        [MemberOrder(200), Optionally, StringLength(15)]
        public virtual string CreditCardApprovalCode { get; set; }

        #endregion

        #region SubTotal (Decimal)

        [MemberOrder(210)]
        public virtual decimal SubTotal { get; set; }

        #endregion

        #region TaxAmt (Decimal)

        [MemberOrder(220)]
        public virtual decimal TaxAmt { get; set; }

        #endregion

        #region Freight (Decimal)

        [MemberOrder(230)]
        public virtual decimal Freight { get; set; }

        #endregion

        #region TotalDue (Decimal)

        [MemberOrder(240)]
        public virtual decimal TotalDue { get; set; }

        #endregion

        #region Comment (String)

        [MemberOrder(250), Optionally, StringLength(128)]
        public virtual string Comment { get; set; }

        #endregion

        #region rowguid (Guid)

        [MemberOrder(260)]
        public virtual Guid rowguid { get; set; }

        #endregion

        #region ModifiedDate (DateTime)

        [MemberOrder(270), Mask("d")]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #endregion

        #region Navigation Properties

        #region Address (Address)

        [MemberOrder(280)]
        public virtual Address Address { get; set; }

        #endregion

        #region Address1 (Address)

        [MemberOrder(290)]
        public virtual Address Address1 { get; set; }

        #endregion

        #region Contact (Contact)

        [MemberOrder(300)]
        public virtual Contact Contact { get; set; }

        #endregion

        #region ShipMethod (ShipMethod)

        [MemberOrder(310)]
        public virtual ShipMethod ShipMethod { get; set; }

        #endregion

        #region CreditCard (CreditCard)

        [MemberOrder(320)]
        public virtual CreditCard CreditCard { get; set; }

        #endregion

        #region CurrencyRate (CurrencyRate)

        [MemberOrder(330)]
        public virtual CurrencyRate CurrencyRate { get; set; }

        #endregion

        #region Customer (Customer)

        [MemberOrder(340)]
        public virtual Customer Customer { get; set; }

        #endregion

        #region SalesOrderDetails (Collection of SalesOrderDetail)

        private ICollection<SalesOrderDetail> _salesOrderDetails = new List<SalesOrderDetail>();

        [MemberOrder(350), Disabled]
        public virtual ICollection<SalesOrderDetail> SalesOrderDetails {
            get { return _salesOrderDetails; }
            set { _salesOrderDetails = value; }
        }

        #endregion

        #region SalesPerson (SalesPerson)

        [MemberOrder(360)]
        public virtual SalesPerson SalesPerson { get; set; }

        #endregion

        #region SalesTerritory (SalesTerritory)

        [MemberOrder(370)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region SalesOrderHeaderSalesReasons (Collection of SalesOrderHeaderSalesReason)

        private ICollection<SalesOrderHeaderSalesReason> _salesOrderHeaderSalesReasons = new List<SalesOrderHeaderSalesReason>();

        [MemberOrder(380), Disabled]
        public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReasons {
            get { return _salesOrderHeaderSalesReasons; }
            set { _salesOrderHeaderSalesReasons = value; }
        }

        #endregion

        #endregion
    }
}