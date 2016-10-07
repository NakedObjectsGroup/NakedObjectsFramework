// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;

namespace AdventureWorksModel {
    [DisplayName("Sales Order")]
    [IconName("trolley.png")]
    public class SalesOrderHeader : ICreditCardCreator {

        #region Injected Servives
        public IDomainObjectContainer Container { set; protected get; }

        public ShoppingCartRepository ShoppingCartRepository { set; protected get; }

        public ProductRepository ProductRepository { set; protected get; }

        public SalesRepository SalesRepository { set; protected get; }

        public PersonRepository PersonRepository { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public void Created() {
            OrderDate = DateTime.Now.Date;
            DueDate = DateTime.Now.Date.AddDays(7);
            RevisionNumber = 1;
            Status = 1;
            SubTotal = 0;
            TaxAmt = 0;
            Freight = 0;
            TotalDue = 0;
        }

        public void Updating() {
            const byte increment = 1;
            RevisionNumber += increment;
            ModifiedDate = DateTime.Now;
        }

        public void Persisted() {
            if (AddItemsFromCart) {
                ShoppingCartRepository.AddAllItemsInCartToOrder(this);
                AddItemsFromCart = false;
            }
        }

        public void Persisting() {
            rowguid = Guid.NewGuid();
            ModifiedDate = DateTime.Now;
        }
        #endregion

        [Disabled, NotPersisted, Hidden(WhenTo.OncePersisted)]
        public bool AddItemsFromCart { get; set; }

        #region Properties

        #region ID

        [NakedObjectsIgnore]
        public virtual int SalesOrderID { get; set; }

        #endregion

        #region SalesOrderNumber

        [Title]
        [Disabled]
        [MemberOrder(1)]
        public virtual string SalesOrderNumber { get; set; }

        #endregion

        #region Status

        //Properly, the Status property should be [Disabled], and modified only through
        //appropriate actions such as Approve.  It has been left modifiable here only
        //to demonstrate the behaviour of Enum properties.
        [MemberOrder(1.1), TypicalLength(12), EnumDataType(typeof (OrderStatus))]
        public virtual byte Status { get; set; }

        public byte DefaultStatus() {
            return (byte) OrderStatus.InProcess;
        }

        [NakedObjectsIgnore]
        public virtual Boolean IsInProcess() {
            return Status.Equals((byte) OrderStatus.InProcess);
        }

        [NakedObjectsIgnore]
        public virtual Boolean IsApproved() {
            return Status.Equals((byte) OrderStatus.Approved);
        }

        [NakedObjectsIgnore]
        public virtual bool IsBackOrdered() {
            return Status.Equals((byte) OrderStatus.BackOrdered);
        }

        [NakedObjectsIgnore]
        public virtual bool IsRejected() {
            return Status.Equals((byte) OrderStatus.Rejected);
        }

        [NakedObjectsIgnore]
        public virtual bool IsShipped() {
            return Status.Equals((byte) OrderStatus.Shipped);
        }

        [NakedObjectsIgnore]
        public virtual bool IsCancelled() {
            return Status.Equals((byte) OrderStatus.Cancelled);
        }

        #endregion

        #region Customer

        [NakedObjectsIgnore]
        public virtual int CustomerID { get; set; }

        [Disabled, MemberOrder(2)]
        public virtual Customer Customer { get; set; }

        #endregion

        #region Contact

        //[NakedObjectsIgnore]
        //public virtual int ContactID { get; set; }

        //private Contact _contact;
        //[NakedObjectsIgnore]
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

        [NakedObjectsIgnore]
        public virtual int BillingAddressID { get; set; }

        [MemberOrder(4)]
        public virtual Address BillingAddress { get; set; }

        [Executed(Where.Remotely)]
        public List<Address> ChoicesBillingAddress() {
            return  PersonRepository.AddressesFor(Customer.BusinessEntity()).ToList();
        }

        #endregion

        #region PurchaseOrderNumber

        [Optionally, StringLength(25), MemberOrder(5)]
        public virtual string PurchaseOrderNumber { get; set; }

        #endregion

        #region ShippingAddress

        [NakedObjectsIgnore]
        public virtual int ShippingAddressID { get; set; }

        [MemberOrder(10)]
        public virtual Address ShippingAddress { get; set; }

        [Executed(Where.Remotely)]
        public List<Address> ChoicesShippingAddress() {
            return ChoicesBillingAddress();
        }
        #endregion

        #region ShipMethod

        [NakedObjectsIgnore]
        public virtual int ShipMethodID { get; set; }

        [MemberOrder(11)]
        public virtual ShipMethod ShipMethod { get; set; }

        public ShipMethod DefaultShipMethod() {
            return Container.Instances<ShipMethod>().FirstOrDefault();
        }

        #endregion

        #region AccountNumber

        [Optionally, StringLength(15), MemberOrder(12)]
        public virtual string AccountNumber { get; set; }

        #endregion

        #region Dates

        #region OrderDate

        [Disabled]
        [MemberOrder(20)]
        public virtual DateTime OrderDate { get; set; }

        #endregion

        #region DueDate

        [MemberOrder(21)]
        public virtual DateTime DueDate { get; set; }

        public string DisableDueDate() {
            return DisableIfShipped();
        }

        public string ValidateDueDate(DateTime dueDate) {
            if (dueDate.Date < OrderDate.Date) {
                return "Due date cannot be before order date";
            }

            return null;
        }

        private string DisableIfShipped() {
            if (IsShipped()) {
                return "Order has been shipped";
            }
            return null;
        }

        #endregion

        #region ShipDate

        [Optionally]
        [MemberOrder(22)]
        [DataType(DataType.DateTime)][Mask("d")]//Just to prove that you can, if perverse enough, make something
        //a dateTime and then mask it as a Date
        [Range(-30, 0)]
        public virtual DateTime? ShipDate { get; set; }

        public string DisableShipDate() {
            return DisableIfShipped();
        }

        public string ValidateShipDate(DateTime? shipDate) {
            if (shipDate.HasValue && shipDate.Value.Date < OrderDate.Date) {
                return "Ship date cannot be before order date";
            }

            return null;
        }

        #endregion

        #endregion

        #region Amounts

        [Disabled]
        [MemberOrder(31)]
        [Mask("C")]
        public virtual decimal SubTotal { get; set; }

        [Disabled]
        [MemberOrder(32)]
        [Mask("C")]
        public virtual decimal TaxAmt { get; set; }

        [Disabled]
        [MemberOrder(33)]
        [Mask("C")]
        public virtual decimal Freight { get; set; }

        [Disabled]
        [MemberOrder(34)]
        [Mask("C")]
        public virtual decimal TotalDue { get; set; }

        public void Recalculate() {
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

        [NakedObjectsIgnore]
        public virtual int? CurrencyRateID { get; set; }

        [Optionally]
        [MemberOrder(35)]
        [FindMenu]
        public virtual CurrencyRate CurrencyRate { get; set; }

        #endregion

        #endregion

        #region OnlineOrder

        [Description("Order has been placed via the web")]
        [Disabled]
        [MemberOrder(41)]
        [DisplayName("Online Order")]
        public virtual bool OnlineOrder { get; set; }

        #endregion

        #region CreditCard
        [NakedObjectsIgnore]
        public virtual int? CreditCardID { get; set; }

        [Optionally]
        [MemberOrder(42)]
        public virtual CreditCard CreditCard { get; set; }

        #endregion

        #region CreditCardApprovalCode

        [Disabled]
        [StringLength(15)]
        [MemberOrder(43)]
        public virtual string CreditCardApprovalCode { get; set; }

        #endregion

        #region RevisionNumber

        [Disabled]
        [MemberOrder(51)]
        public virtual byte RevisionNumber { get; set; }

        #endregion

        #region Comment

        [Optionally]
        [MultiLine(NumberOfLines = 3, Width = 50)]
        [MemberOrder(52)]
        [Description("Free-form text")]
        public virtual string Comment { get; set; }


        public bool HideComment()
        {
            return Comment == null || Comment == "";
        }

        public void ClearComment()
        {
            this.Comment = null;
        }

        public void AddStandardComments(IEnumerable<string> comments) {
            foreach (string comment in comments) {
                Comment += comment + "\n";
            }
        }

        public string[] Choices0AddStandardComments() {
            return new[] {
                "Payment on delivery",
                "Leave parcel with neighbour",
                "Leave parcel round back"
            };
        }

        public string[] Default0AddStandardComments() {
            return new[] {
                "Payment on delivery",
                "Leave parcel with neighbour"
            };
        }

        //Action to demonstrate use of auto-complete that returns IEnumerable<string>
        public void AddComment(string comment) {
            Comment += comment + "\n";
        }

        [PageSize(10)]
        public IEnumerable<string> AutoComplete0AddComment(
            [DescribedAs("Auto-complete")] [MinLength(2)] string matching) {
            return Choices0AddStandardComments().Where(c => c.ToLower().Contains(matching.ToLower()));
        }

        //Action to demonstrate use of auto-complete that returns IEnumerable<string>
        public void AddComment2(string comment) {
            Comment += comment + "\n";
        }

        [PageSize(10)]
        public IList<string> AutoComplete0AddComment2(
            [DescribedAs("Auto-complete")] [MinLength(2)] string matching) {
            return Choices0AddStandardComments().Where(c => c.ToLower().Contains(matching.ToLower())).ToList();
        }

        public void AddMultiLineComment([MultiLine(NumberOfLines = 3)] string comment)
        {
            Comment += comment + "\n";
        }
        #endregion

        #region SalesPerson

        [NakedObjectsIgnore]
        public virtual int? SalesPersonID { get; set; }

        [Optionally]
        [MemberOrder(61)]
        public virtual SalesPerson SalesPerson { get; set; }

        [PageSize(20)]
        public IQueryable<SalesPerson> AutoCompleteSalesPerson([MinLength(2)] string name) {
            return SalesRepository.FindSalesPersonByName(null, name);
        }

        #endregion

        #region SalesTerritory
        [NakedObjectsIgnore]
        public virtual int? SalesTerritoryID { get; set; }

        [Optionally]
        [MemberOrder(62)]
        public virtual SalesTerritory SalesTerritory { get; set; }

        #endregion

        #region ModifiedDate and rowguid

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        #region rowguid

        [NakedObjectsIgnore]
        public virtual Guid rowguid { get; set; }

        #endregion

        #endregion

        #endregion

        #region Collections

        #region Details

        private ICollection<SalesOrderDetail> details = new List<SalesOrderDetail>();

        [Disabled]
        public virtual ICollection<SalesOrderDetail> Details {
            get { return details; }
            set { details = value; }
        }

        #endregion

        #region Reasons

        private ICollection<SalesOrderHeaderSalesReason> salesOrderHeaderSalesReason = new List<SalesOrderHeaderSalesReason>();

        [Disabled]
        [DisplayName("Reasons")]
        public virtual ICollection<SalesOrderHeaderSalesReason> SalesOrderHeaderSalesReason {
            get { return salesOrderHeaderSalesReason; }
            set { salesOrderHeaderSalesReason = value; }
        }

        #endregion

        #endregion

        #region Actions

        #region Add New Detail

        [Description("Add a new line item to the order")]
#pragma warning disable 612,618
        [MemberOrder(Sequence="2", Name ="Details")]
#pragma warning restore 612,618
        public SalesOrderDetail AddNewDetail(Product product,
                                             [DefaultValue((short) 1), Range(1, 999)] short quantity) {
            int stock = product.NumberInStock();
            if (stock < quantity) {
                var t = Container.NewTitleBuilder();
                t.Append("Current inventory of").Append(product).Append(" is").Append(stock);
                Container.WarnUser(t.ToString());
            }
            var sod = Container.NewTransientInstance<SalesOrderDetail>();
            sod.SalesOrderHeader = this;
            sod.SalesOrderID = SalesOrderID;
            sod.OrderQty = quantity;
            sod.SpecialOfferProduct = product.BestSpecialOfferProduct(quantity);
            sod.Recalculate();

            return sod;
        }

        public virtual string DisableAddNewDetail() {
            if (!IsInProcess()) {
                return "Can only add to 'In Process' order";
            }
            return null;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0AddNewDetail([MinLength(2)] string name) {
            return ProductRepository.FindProductByName(name);
        }
        #endregion

        #region Add New Details
        [Description("Add multiple line items to the order")]
        [MultiLine()]
#pragma warning disable 612, 618
        [MemberOrder(Sequence = "1", Name = "Details")]
#pragma warning restore 612,618
        public void AddNewDetails(Product product,
                                     [DefaultValue((short)1)] short quantity)
        {
            var detail = AddNewDetail(product, quantity);
            Container.Persist(ref detail);
        }
        public virtual string DisableAddNewDetails()
        {
            return DisableAddNewDetail();
        }
        [PageSize(20)]
        public IQueryable<Product> AutoComplete0AddNewDetails([MinLength(2)] string name)
        {
            return AutoComplete0AddNewDetail(name);
        }

        public string ValidateAddNewDetails(short quantity)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(quantity <= 0, "Must be > 0");
            return rb.Reason;
        }

        #endregion

        #region Remove Detail
        public void RemoveDetail(SalesOrderDetail detailToRemove) {
            if (Details.Contains(detailToRemove)) {
                Details.Remove(detailToRemove);
            }
        }

        public IEnumerable<SalesOrderDetail> Choices0RemoveDetail() {
            return Details;
        }

        public SalesOrderDetail Default0RemoveDetail() {
            return Details.FirstOrDefault();
        }

        [MemberOrder(3)]
        public void RemoveDetails([ContributedAction] IEnumerable<SalesOrderDetail> details) {
            foreach (SalesOrderDetail detail in details) {
                if (Details.Contains(detail)) {
                    Details.Remove(detail);
                }
            }
        }
        #endregion

        #region AdjustQuantities
        [MemberOrder(4)]
        public void AdjustQuantities([ContributedAction] IEnumerable<SalesOrderDetail> details, short newQuantity)
        {
            foreach (SalesOrderDetail detail in details)
            {
                detail.OrderQty = newQuantity;
            }
        }

        public string ValidateAdjustQuantities(IEnumerable<SalesOrderDetail> details, short newQuantity)
        {
            var rb = new ReasonBuilder();
            rb.AppendOnCondition(details.Count(d => d.OrderQty == newQuantity) == details.Count(),
                "All selected details already have specified quantity");
            return rb.Reason;
        }

        #endregion

        #region CreateNewCreditCard

        [NakedObjectsIgnore]
        public void CreatedCardHasBeenSaved(CreditCard card) {
            CreditCard = card;
        }

        #endregion

        #region AddNewSalesReason

#pragma warning disable 618
        [MemberOrder(Name= "SalesOrderHeaderSalesReason", Sequence = "1")]
#pragma warning restore 618
        public void AddNewSalesReason(SalesReason reason) {
            if (SalesOrderHeaderSalesReason.All(y => y.SalesReason != reason)) {
                var link = Container.NewTransientInstance<SalesOrderHeaderSalesReason>();
                link.SalesOrderHeader = this;
                link.SalesReason = reason;
                Container.Persist(ref link);
                SalesOrderHeaderSalesReason.Add(link);
            }
            else {
                Container.WarnUser(string.Format("{0} already exists in Sales Reasons", reason.Name));
            }
        }

        public string ValidateAddNewSalesReason(SalesReason reason) {
            return SalesOrderHeaderSalesReason.Any(y => y.SalesReason == reason) ? string.Format("{0} already exists in Sales Reasons", reason.Name) : null;
        }

        [MemberOrder(1)]
        public void RemoveSalesReason(SalesOrderHeaderSalesReason reason)
        {
            this.SalesOrderHeaderSalesReason.Remove(reason);
            Container.DisposeInstance(reason);
        }


        public IList<SalesOrderHeaderSalesReason> Choices0RemoveSalesReason()
        {
            return this.SalesOrderHeaderSalesReason.ToList();
        }

        [MemberOrder(2)]
        public void AddNewSalesReasons(IEnumerable<SalesReason> reasons) {
            foreach (SalesReason r in reasons) {
                AddNewSalesReason(r);
            }
        }

        public string ValidateAddNewSalesReasons(IEnumerable<SalesReason> reasons) {
            return reasons.Select(ValidateAddNewSalesReason).Aggregate("", (s, t) => string.IsNullOrEmpty(s) ? t : s + ", " + t);
        }

        [MemberOrder(2)]
        public void RemoveSalesReasons(
            [ContributedAction] IEnumerable<SalesOrderHeaderSalesReason> salesOrderHeaderSalesReason)
        {
            foreach(var reason in salesOrderHeaderSalesReason)
            {
                this.RemoveSalesReason(reason);
            }
        }

        // This is done with an enum in order to test enum parameter handling by the framework
        public enum SalesReasonCategories {
            Other,
            Promotion,
            Marketing
        }

        [MemberOrder(3)]
        public void AddNewSalesReasonsByCategory(SalesReasonCategories reasonCategory) {
            IQueryable<SalesReason> allReasons = Container.Instances<SalesReason>();
            var catName = reasonCategory.ToString();

            foreach (SalesReason salesReason in allReasons.Where(salesReason => salesReason.ReasonType == catName)) {
                AddNewSalesReason(salesReason);
            }
        }

        [MemberOrder(4)]
        public void AddNewSalesReasonsByCategories(IEnumerable<SalesReasonCategories> reasonCategories) {
            foreach (SalesReasonCategories rc in reasonCategories) {
                AddNewSalesReasonsByCategory(rc);
            }
        }

        // Use 'hide', 'dis', 'val', 'actdef', 'actcho' shortcuts to add supporting methods here.

        #endregion

        #region ApproveOrder

        [MemberOrder(1)]
        public void ApproveOrder() {
            Status = (byte) OrderStatus.Approved;
        }

        public virtual bool HideApproveOrder() {
            return !IsInProcess();
        }

        public virtual string DisableApproveOrder() {
            var rb = new ReasonBuilder();
            if (Details.Count == 0) {
                rb.Append("Cannot approve orders with no Details");
            }
            return rb.Reason;
        }

        #endregion

        #region MarkAsShipped

        [Description("Indicate that the order has been shipped, specifying the date")]
        [Hidden(WhenTo.Always)] //Testing that the complementary methods don't show up either
        public void MarkAsShipped(DateTime shipDate) {
            Status = (byte) OrderStatus.Shipped;
            ShipDate = shipDate;
        }

        public virtual string ValidateMarkAsShipped(DateTime date) {
            var rb = new ReasonBuilder();
            if (date.Date > DateTime.Now.Date) {
                rb.Append("Ship Date cannot be after Today");
            }
            return rb.Reason;
        }

        public virtual string DisableMarkAsShipped() {
            if (!IsApproved()) {
                return "Order must be Approved before shipping";
            }
            return null;
        }

        public virtual bool HideMarkAsShipped() {
            return IsRejected() || IsShipped() || IsCancelled();
        }

        #endregion

        #region CancelOrder

        // return this for testing purposes
        public SalesOrderHeader CancelOrder() {
            Status = (byte) OrderStatus.Cancelled;
            return this;
        }

        public virtual bool HideCancelOrder() {
            return IsShipped() || IsCancelled();
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