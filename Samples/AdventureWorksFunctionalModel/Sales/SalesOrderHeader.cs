// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFunctions;

namespace AdventureWorksModel {
    [Named("Sales Order")]
    public record SalesOrderHeader : ICreditCardCreator
    {
        public bool AddItemsFromCart { get; set; }

        #region Properties

        #region ID

        [Hidden]
        public virtual int SalesOrderID { get; set; }

        #endregion

        #region SalesOrderNumber

        //Title

        [MemberOrder(1)]
        public virtual string SalesOrderNumber { get; set; }

        #endregion

        #region Status

        //Properly, the Status property should be , and modified only through
        //appropriate actions such as Approve.  It has been left modifiable here only
        //to demonstrate the behaviour of Enum properties.
        [MemberOrder(1), EnumDataType(typeof(OrderStatus))]
        public virtual byte Status { get; set; }

        public byte DefaultStatus()
        {
            return (byte)OrderStatus.InProcess;
        }

        [Hidden]
        public virtual bool IsInProcess()
        {
            return Status.Equals((byte)OrderStatus.InProcess);
        }

        [Hidden]
        public virtual bool IsApproved()
        {
            return Status.Equals((byte)OrderStatus.Approved);
        }

        [Hidden]
        public virtual bool IsBackOrdered()
        {
            return Status.Equals((byte)OrderStatus.BackOrdered);
        }

        [Hidden]
        public virtual bool IsRejected()
        {
            return Status.Equals((byte)OrderStatus.Rejected);
        }

        [Hidden]
        public virtual bool IsShipped()
        {
            return Status.Equals((byte)OrderStatus.Shipped);
        }

        [Hidden]
        public virtual bool IsCancelled()
        {
            return Status.Equals((byte)OrderStatus.Cancelled);
        }

        #endregion

        #region Customer

        [Hidden]
        public virtual int CustomerID { get; set; }

        [MemberOrder(2)]
        public virtual Customer Customer { get; set; }

        #endregion

        #region Contact

        //[Hidden]
        //public virtual int ContactID { get; set; }

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

        //[Executed(Where.Remotely)]
        //public List<StoreContact> ChoicesStoreContact() {
        //    throw new NotImplementedException();
        //    //if (Customer != null && Customer.IsStore()) {
        //    //    return new List<StoreContact>(((Store) Customer).Contacts);
        //    //}
        //    //return new List<StoreContact>();
        //}
        //#endregion

        #endregion

        #region BillingAddress

        [Hidden]
        public virtual int BillingAddressID { get; set; }

        [MemberOrder(4)]
        public virtual Address BillingAddress { get; set; }

        [Executed(Where.Remotely)]
        public List<Address> ChoicesBillingAddress(IQueryable<BusinessEntityAddress> addresses)
        {
            return PersonRepository.AddressesFor(Customer.BusinessEntity(), addresses).ToList();
        }

        #endregion

        #region PurchaseOrderNumber

        [MemberOrder(5)]
        public virtual string PurchaseOrderNumber { get; set; }

        #endregion

        #region ShippingAddress

        [Hidden]
        public virtual int ShippingAddressID { get; set; }

        [MemberOrder(10)]
        public virtual Address ShippingAddress { get; set; }

        [Executed(Where.Remotely)]
        public List<Address> ChoicesShippingAddress(IQueryable<BusinessEntityAddress> addresses)
        {
            return ChoicesBillingAddress(addresses);
        }
        #endregion

        #region ShipMethod

        [Hidden]
        public virtual int ShipMethodID { get; set; }

        [MemberOrder(11)]
        public virtual ShipMethod ShipMethod { get; set; }

        public ShipMethod DefaultShipMethod()
        {
            return Container.Instances<ShipMethod>().FirstOrDefault();
        }

        #endregion

        #region AccountNumber

        [Optionally, MemberOrder(12)]
        public virtual string AccountNumber { get; set; }

        #endregion

        #region Dates

        #region OrderDate


        [MemberOrder(20)]
        public virtual DateTime OrderDate { get; set; }

        #endregion

        #region DueDate

        [MemberOrder(21)]
        public virtual DateTime DueDate { get; set; }

        public string DisableDueDate()
        {
            return DisableIfShipped();
        }

        public string ValidateDueDate(DateTime dueDate)
        {
            if (dueDate.Date < OrderDate.Date)
            {
                return "Due date cannot be before order date";
            }

            return null;
        }

        private string DisableIfShipped()
        {
            if (IsShipped())
            {
                return "Order has been shipped";
            }
            return null;
        }

        #endregion

        #region ShipDate


        [MemberOrder(22)]
        [DataType(DataType.DateTime)]
        [Mask("d")]//Just to prove that you can, if perverse enough, make something
        //a dateTime and then mask it as a Date
        [Range(-30, 0)]
        public virtual DateTime? ShipDate { get; set; }

        public string DisableShipDate()
        {
            return DisableIfShipped();
        }

        public string ValidateShipDate(DateTime? shipDate)
        {
            if (shipDate.HasValue && shipDate.Value.Date < OrderDate.Date)
            {
                return "Ship date cannot be before order date";
            }

            return null;
        }

        #endregion

        #endregion

        #region Amounts


        [MemberOrder(31)]
        [Mask("C")]
        public virtual decimal SubTotal { get; set; }


        [MemberOrder(32)]
        [Mask("C")]
        public virtual decimal TaxAmt { get; set; }


        [MemberOrder(33)]
        [Mask("C")]
        public virtual decimal Freight { get; set; }


        [MemberOrder(34)]
        [Mask("C")]
        public virtual decimal TotalDue { get; set; }

        public void Recalculate()
        {
            SubTotal = Details.Sum(d => d.LineTotal);
            TotalDue = SubTotal;
        }

        public virtual string DisableRecalculate()
        {
            if (!IsInProcess())
            {
                return "Can only recalculate an 'In Process' order";
            }
            return null;
        }

        #region CurrencyRate

        [Hidden]
        public virtual int? CurrencyRateID { get; set; }


        [MemberOrder(35)]
        [FindMenu]
        public virtual CurrencyRate CurrencyRate { get; set; }

        #endregion

        #endregion

        #region OnlineOrder

        [DescribedAs("Order has been placed via the web")]

        [MemberOrder(41)]
        [Named("Online Order")]
        public virtual bool OnlineOrder { get; set; }

        #endregion

        #region CreditCard
        [Hidden]
        public virtual int? CreditCardID { get; set; }


        [MemberOrder(42)]
        public virtual CreditCard CreditCard { get; set; }

        #endregion

        #region CreditCardApprovalCode



        [MemberOrder(43)]
        public virtual string CreditCardApprovalCode { get; set; }

        #endregion

        #region RevisionNumber


        [MemberOrder(51)]
        public virtual byte RevisionNumber { get; set; }

        #endregion

        #region Comment


        [MultiLine(NumberOfLines = 3, Width = 50)]
        [MemberOrder(52)]
        [DescribedAs("Free-form text")]
        public virtual string Comment { get; set; }


        public bool HideComment()
        {
            return Comment == null || Comment == "";
        }

        public void ClearComment()
        {
            this.Comment = null;
        }

        public void AddStandardComments(IEnumerable<string> comments)
        {
            foreach (string comment in comments)
            {
                Comment += comment + "\n";
            }
        }

        public string[] Choices0AddStandardComments()
        {
            return new[] {
                "Payment on delivery",
                "Leave parcel with neighbour",
                "Leave parcel round back"
            };
        }

        public string[] Default0AddStandardComments()
        {
            return new[] {
                "Payment on delivery",
                "Leave parcel with neighbour"
            };
        }

        //Action to demonstrate use of auto-complete that returns IEnumerable<string>
        public void AddComment(string comment)
        {
            Comment += comment + "\n";
        }

        [PageSize(10)]
        public IEnumerable<string> AutoComplete0AddComment(
            [DescribedAs("Auto-complete")][Range(2, 0)] string matching)
        {
            return Choices0AddStandardComments().Where(c => c.ToLower().Contains(matching.ToLower()));
        }

        //Action to demonstrate use of auto-complete that returns IEnumerable<string>
        public void AddComment2(string comment)
        {
            Comment += comment + "\n";
        }

        [PageSize(10)]
        public IList<string> AutoComplete0AddComment2(
            [DescribedAs("Auto-complete")][Range(2, 0)] string matching)
        {
            return Choices0AddStandardComments().Where(c => c.ToLower().Contains(matching.ToLower())).ToList();
        }

        public void AddMultiLineComment([MultiLine(NumberOfLines = 3)] string comment)
        {
            Comment += comment + "\n";
        }
        #endregion

        #region SalesPerson

        [Hidden]
        public virtual int? SalesPersonID { get; set; }


        [MemberOrder(61)]
        public virtual SalesPerson SalesPerson { get; set; }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoCompleteSalesPerson(
            [Range(2, 0)] string name,
            IQueryable<Person> persons,
            IQueryable<SalesPerson> sps)
        {
            return SalesRepository.FindSalesPersonByName(null, name, persons, sps);
        }

        #endregion

        #region SalesTerritory
        [Hidden]
        public virtual int? SalesTerritoryID { get; set; }


        [MemberOrder(62)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]

        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [Hidden]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion

        #endregion

        #region Collections

        #region Details

        private ICollection<SalesOrderDetail> details = new List<SalesOrderDetail>();


        public virtual ICollection<SalesOrderDetail> Details
        {
            get { return details; }
            set { details = value; }
        }

        #endregion

        #region Reasons

        private ICollection<SalesOrderHeaderSalesReason> salesOrderHeaderSalesReason = new List<SalesOrderHeaderSalesReason>();


        [Named("Reasons")]
        public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReason
        {
            get { return salesOrderHeaderSalesReason; }
            set { salesOrderHeaderSalesReason = value; }
        }

        #endregion

        #endregion

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