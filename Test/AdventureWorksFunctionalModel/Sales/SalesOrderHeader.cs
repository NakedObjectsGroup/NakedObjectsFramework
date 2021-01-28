// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Functions;
using NakedFunctions;
using static AW.Utilities;

namespace AW.Types {
    [Named("Sales Order")]
    public record SalesOrderHeader : ICreditCardCreator
    {
        public virtual bool AddItemsFromCart { get; init; }

        [Hidden]
        public virtual int SalesOrderID { get; init; }

        [MemberOrder(1)]
        public virtual string SalesOrderNumber { get; init; }

        [Hidden]
        public virtual byte StatusByte { get; init; }

        [MemberOrder(1)]
        public virtual OrderStatus Status { get { return (OrderStatus)StatusByte;  } }

        [Hidden]
        public virtual int CustomerID { get; init; }

        [MemberOrder(2)]
        public virtual Customer Customer { get; init; }

        //[Hidden]
        //public virtual int ContactID { get; init; }

        //private Contact _contact;
        //[Hidden]
        //public virtual Contact Contact { get { return _contact; } set { _contact = value; } }

        //internal void SetUpContact(Contact value) {
        //    Contact = value;
        //    storeContact = FindStoreContactForContact();
        //}

        //#region StoreContact Property

        //private StoreContact storeContact;

        //[MemberOrder(3)]
        //public virtual StoreContact StoreContact {
        //    get { return storeContact; }
        //    set {
        //        if (value != null) {
        //            storeContact = value;
        //            Contact = value.Contact;
        //        }
        //    }
        //}

        //private StoreContact FindStoreContactForContact() {
        //    IQueryable<StoreContact> query = from obj in Container.Instances<StoreContact>()
        //        where obj.Contact.BusinessEntityID == Contact.BusinessEntityID && obj.Store.BusinessEntityID == Customer.Store.BusinessEntityID
        //        select obj;

        //    return query.FirstOrDefault();
        //}

        //public virtual bool HideStoreContact() {
        //    return Customer != null && Customer.IsIndividual();
        //}


        //public List<StoreContact> ChoicesStoreContact() {
        //    throw new NotImplementedException();
        //    //if (Customer != null && Customer.IsStore()) {
        //    //    return new List<StoreContact>(((Store) Customer).Contacts);
        //    //}
        //    //return new List<StoreContact>();
        //}
        //#endregion

        [Hidden]
        public virtual int BillingAddressID { get; init; }

        [MemberOrder(4)]
        public virtual Address BillingAddress { get; init; }


        [MemberOrder(5)]
        public virtual string PurchaseOrderNumber { get; init; }

        [Hidden]
        public virtual int ShippingAddressID { get; init; }

        [MemberOrder(10)]
        public virtual Address ShippingAddress { get; init; }

        [Hidden]
        public virtual int ShipMethodID { get; init; }

        [MemberOrder(11)]
        public virtual ShipMethod ShipMethod { get; init; }

        [Optionally, MemberOrder(12)]
        public virtual string AccountNumber { get; init; }

        [MemberOrder(20)]
        public virtual DateTime OrderDate { get; init; }

        [MemberOrder(21)]
        public virtual DateTime DueDate { get; init; }

        [MemberOrder(22), Mask("d")]
       // [NakedFunctions.Range(-30, 0)]
        public virtual DateTime? ShipDate { get; init; }
  
        [MemberOrder(31)]
        [Mask("C")]
        public virtual decimal SubTotal { get; init; }

        [MemberOrder(32), Mask("C")]
        public virtual decimal TaxAmt { get; init; }

        [MemberOrder(33), Mask("C")]
        public virtual decimal Freight { get; init; }

        [MemberOrder(34), Mask("C")]
        public virtual decimal TotalDue { get; init; }

        [Hidden]
        public virtual int? CurrencyRateID { get; init; }

        [MemberOrder(35)]
        public virtual CurrencyRate CurrencyRate { get; init; }

        [DescribedAs("Order has been placed via the web")]
        [MemberOrder(41),Named("Online Order")]
        public virtual bool OnlineOrder { get; init; }

        [Hidden]
        public virtual int? CreditCardID { get; init; }

        [MemberOrder(42)]
        public virtual CreditCard CreditCard { get; init; }

        [MemberOrder(43)]
        public virtual string CreditCardApprovalCode { get; init; }


        [MemberOrder(51)]
        public virtual byte RevisionNumber { get; init; }


        [MultiLine(NumberOfLines = 3, Width = 50)]
        [MemberOrder(52)]
        [DescribedAs("Free-form text")]
        public virtual string Comment { get; init; }


        [Hidden]
        public virtual int? SalesPersonID { get; init; }


        [MemberOrder(61)]
        public virtual SalesPerson SalesPerson { get; init; }


        [Hidden]
        public virtual int? SalesTerritoryID { get; init; }


        [MemberOrder(62)]
        public virtual SalesTerritory SalesTerritory { get; init; }


        [MemberOrder(99)]     
        //[Versioned]
		public virtual DateTime ModifiedDate { get; init; }

        [Hidden]
        public virtual Guid rowguid { get; init; }


        public virtual ICollection<SalesOrderDetail> Details { get; init; } 
            = new List<SalesOrderDetail>();

        [Named("Reasons")]
        public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReason { get; init; } 
            = new List<SalesOrderHeaderSalesReason>();


        public override string ToString() => $"{SalesOrderNumber}";

		public override int GetHashCode() =>base.GetHashCode();

        public virtual bool Equals(SalesOrderHeader other) => ReferenceEquals(this, other);
    }

    public enum OrderStatus : byte {
        InProcess = 1,
        Approved = 2,
        BackOrdered = 3,
        Rejected = 4,
        Shipped = 5,
        Cancelled = 6
    }
}