// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using AW.Types;
using System.Linq;
using NakedFunctions;
using System.Collections.Generic;

namespace AW.Functions {
    [Named("Sales Order")]
        public  static class SalesOrderHeader_Functions {

        #region Add New Detail
        [MemberOrder(2), DescribedAs("Add a new line item to the order")]
        public static IContext AddNewDetail(
                this SalesOrderHeader soh,
                Product product,
                [DefaultValue(1), ValueRange(1, 999)] short quantity,
                IContext context
            )
        {            
            SalesOrderDetail sod = CreateNewDetail(soh, product, quantity, context);
            return context.WithNew(sod).WithDeferred(c => {
                var soh2 = c.Reload(soh);
                return c.WithUpdated(soh2, soh2.Recalculated(c));
            });
        }

        public static string DisableAddNewDetail(this SalesOrderHeader soh)
        {
            if (!soh.IsInProcess())
            {
                return "Can only add to 'In Process' order";
            }
            return null;
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AddNewDetail(this SalesOrderHeader soh,
            [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);

        #endregion

        internal static SalesOrderDetail CreateNewDetail(this SalesOrderHeader soh, Product product, short quantity, IContext context)
        {
            var specialOfferProduct = Product_Functions.BestSpecialOfferProduct(product, quantity, context);
            return new SalesOrderDetail()
            {
                SalesOrderHeader = soh,
                OrderQty = quantity,
                SpecialOfferProduct = specialOfferProduct,
                Product = product,
                UnitPrice = product.ListPrice,
                UnitPriceDiscount = specialOfferProduct.SpecialOffer.DiscountPct,
                rowguid = context.NewGuid(),
                ModifiedDate = context.Now()
            };
        }

        internal static SalesOrderHeader Recalculated(this SalesOrderHeader soh, IContext c)
        {
            var subTotal = soh.Details.Any() ? soh.Details.Sum(d => d.LineTotal) : 0.0m;
            var tax = subTotal * soh.GetTaxRate(c) / 100;
            var total = subTotal + tax;
            return soh with
            {
                SubTotal = subTotal,
                TaxAmt = tax,
                TotalDue = total,
                ModifiedDate = c.Now()
            };
        }

        internal static decimal GetTaxRate(this SalesOrderHeader soh, IContext context)
        {
            var stateId = soh.BillingAddress.StateProvince.StateProvinceID;
            var str = context.Instances<SalesTaxRate>().FirstOrDefault(str => str.StateProvinceID == stateId);
            return str is null ? 0 : str.TaxRate;
        }



        //        #region Add New Details
        //        [DescribedAs("Add multiple line items to the order")]
        //        [MultiLine()]
        //#pragma warning disable 612, 618
        //        [MemberOrder(1, "Details")]
        //#pragma warning restore 612,618
        //        public void AddNewDetails(
        //            Product product,
        //           [DefaultValue((short)1)] short quantity,
        //           IQueryable<SpecialOfferProduct> sops)
        //        {
        //            var detail = AddNewDetail(product, quantity, sops);
        //            Container.Persist(ref detail);
        // TODO: Call this at the end
        //SalesOrderHeader.Recalculate()

        //        }
        //        public virtual string DisableAddNewDetails()
        //        {
        //            return DisableAddNewDetail();
        //        }
        //        [PageSize(20)]
        //        public IQueryable<Product> AutoComplete0AddNewDetails([MinLength(2)] string name, IQueryable<Product> products)
        //        {
        //            return AutoComplete0AddNewDetail(name, products);
        //        }

        //        public string ValidateAddNewDetails(short quantity)
        //        {
        //            var rb = new ReasonBuilder();
        //            rb.AppendOnCondition(quantity <= 0, "Must be > 0");
        //            return rb.Reason;
        //        }

        //        #endregion

        #region Remove Details
        public static IContext RemoveDetail(this SalesOrderHeader soh,
            SalesOrderDetail detailToRemove, IContext context) =>
                     context.WithDeleted(detailToRemove)
                     .WithDeferred(c => c.WithUpdated(soh, soh.Recalculated(c)));


        public static IEnumerable<SalesOrderDetail> Choices1RemoveDetail(this SalesOrderHeader soh) =>
            soh.Details;

        public static SalesOrderDetail Default1RemoveDetail(this SalesOrderHeader soh) =>
            soh.Details.First();

        public static string DisableRemoveDetail(this SalesOrderHeader soh) =>
            soh.Details.Any() ? null : "Order has no Details.";

        [MemberOrder(3)]
        public static  IContext RemoveDetails(this SalesOrderHeader soh,
             IEnumerable<SalesOrderDetail> details, IContext context) =>
                 details.Aggregate(context, (c, d) => c.WithDeleted(d))
                    .WithDeferred(c => c.WithUpdated(soh, soh.Recalculated(c)));

        #endregion

        public static IContext AddCarrierTrackingNumber(this SalesOrderHeader soh,
           IEnumerable<SalesOrderDetail> details, string ctn, IContext context) =>
             details.Select(d => new
             {
                 original = d,
                 updated = d with
                 { CarrierTrackingNumber = ctn, ModifiedDate = context.Now() }
             })
            .Aggregate(context, (c, u) => c.WithUpdated(u.original, u.updated));


        [MemberOrder("Details",1)] //Places action within the Details collection
        public static IContext ChangeAQuantity(this SalesOrderHeader soh, 
            SalesOrderDetail detail, short newQuantity, IContext context) =>
                 detail.ChangeQuantity(newQuantity, context);

        public static List<SalesOrderDetail> Choices1ChangeAQuantity(this SalesOrderHeader soh) =>
            soh.Details.ToList();

        //        #region CreateNewCreditCard

        //        [Hidden]
        //        public void CreatedCardHasBeenSaved(CreditCard card) {
        //            CreditCard = card;
        //        }

        //        #endregion

        //        #region AddNewSalesReason

        //#pragma warning disable 618
        //        [MemberOrder(Name= "SalesOrderHeaderSalesReason", Sequence = "1")]
        //#pragma warning restore 618
        //        public void AddNewSalesReason(SalesReason reason) {
        //            if (SalesOrderHeaderSalesReason.All(y => y.SalesReason != reason)) {
        //                var link = Container.NewTransientInstance<SalesOrderHeaderSalesReason>();
        //                link.SalesOrderHeader = this;
        //                link.SalesReason = reason;
        //                Container.Persist(ref link);
        //                SalesOrderHeaderSalesReason.Add(link);
        //            }
        //            else {
        //                Container.WarnUser(string.Format("{0} already exists in Sales Reasons", reason.Name));
        //            }
        //        }

        //        public string ValidateAddNewSalesReason(SalesReason reason) {
        //            return SalesOrderHeaderSalesReason.Any(y => y.SalesReason == reason) ? string.Format("{0} already exists in Sales Reasons", reason.Name) : null;
        //        }

        //        [MemberOrder(1)]
        //        public void RemoveSalesReason(SalesOrderHeaderSalesReason reason)
        //        {
        //            this.SalesOrderHeaderSalesReason.Remove(reason);
        //            Container.DisposeInstance(reason);
        //        }


        //        public IList<SalesOrderHeaderSalesReason> 
        //Choices0RemoveSalesReason()
        //        {
        //            return this.SalesOrderHeaderSalesReason.ToList();
        //        }

        //        [MemberOrder(2)]
        //        public void AddNewSalesReasons(IEnumerable<SalesReason> reasons) {
        //            foreach (SalesReason r in reasons) {
        //                AddNewSalesReason(r);
        //            }
        //        }

        //        public string ValidateAddNewSalesReasons(IEnumerable<SalesReason> reasons) {
        //            return reasons.Select(ValidateAddNewSalesReason).Aggregate("", (s, t) => string.IsNullOrEmpty(s) ? t : s + ", " + t);
        //        }

        //        [MemberOrder(2)]
        //        public void RemoveSalesReasons(
        //            this IEnumerable<SalesOrderHeaderSalesReason> salesOrderHeaderSalesReason)
        //        {
        //            foreach(var reason in salesOrderHeaderSalesReason)
        //            {
        //                this.RemoveSalesReason(reason);
        //            }
        //        }

        //        // This is done with an enum in order to test enum parameter handling by the framework
        //        public enum SalesReasonCategories {
        //            Other,
        //            Promotion,
        //            Marketing
        //        }

        //        [MemberOrder(3)]
        //        public void AddNewSalesReasonsByCategory(SalesReasonCategories reasonCategory) {
        //            IQueryable<SalesReason> allReasons = Container.Instances<SalesReason>();
        //            var catName = reasonCategory.ToString();

        //            foreach (SalesReason salesReason in allReasons.Where(salesReason => salesReason.ReasonType == catName)) {
        //                AddNewSalesReason(salesReason);
        //            }
        //        }

        //        [MemberOrder(4)]
        //        public void AddNewSalesReasonsByCategories(IEnumerable<SalesReasonCategories> reasonCategories) {
        //            foreach (SalesReasonCategories rc in reasonCategories) {
        //                AddNewSalesReasonsByCategory(rc);
        //            }
        //        }

        //        // Use 'hide', 'dis', 'val', 'actdef', 'actcho' shortcuts to add supporting methods here.

        //        #endregion

        internal static IContext UpdateOrder(
            SalesOrderHeader original, SalesOrderHeader updated, IContext context) =>
                 context.WithUpdated(original, updated with { ModifiedDate = context.Now() });

        #region ApproveOrder

        [MemberOrder(1)]
        public static  IContext ApproveOrder(this SalesOrderHeader soh, IContext context) =>
            UpdateOrder(soh, soh with { StatusByte = (byte)OrderStatus.Approved }, context);
       
        //TODO: Remove context param from next 2
        public static bool HideApproveOrder( this SalesOrderHeader soh, IContext context) =>
          !soh.IsInProcess();

        public static string DisableApproveOrder(this SalesOrderHeader soh, IContext context) =>
            soh.Details.Count == 0 ? "Cannot approve orders with no Details" : null;

        #endregion

        //        #region MarkAsShipped

        //        [DescribedAs("Indicate that the order has been shipped, specifying the date")]
        //        [Hidden] //Testing that the complementary methods don't show up either
        //        public void MarkAsShipped(DateTime shipDate) {
        //            Status = (byte) OrderStatus.Shipped;
        //            ShipDate = shipDate;
        //        }

        //        public virtual string ValidateMarkAsShipped(DateTime date) {
        //            var rb = new ReasonBuilder();
        //            if (date.Date > DateTime.Now.Date) {
        //                rb.Append("Ship Date cannot be after Today");
        //            }
        //            return rb.Reason;
        //        }

        //        public virtual string DisableMarkAsShipped() {
        //            if (!IsApproved()) {
        //                return "Order must be Approved before shipping";
        //            }
        //            return null;
        //        }

        //        public virtual bool HideMarkAsShipped() {
        //            return IsRejected() || IsShipped() || IsCancelled();
        //        }

        //        #endregion

        //        #region CancelOrder

        //        // return this for testing purposes
        //        public SalesOrderHeader CancelOrder() {
        //            Status = (byte) OrderStatus.Cancelled;
        //            return this;
        //        }

        //        public virtual bool HideCancelOrder() {
        //            return IsShipped() || IsCancelled();
        //        }

        //        #endregion


        #region Status helpers
        internal static bool IsInProcess(this SalesOrderHeader soh)
        {
            return soh.StatusByte == 1; //OrderStatus.InProcess;
        }

        internal static bool IsApproved(this SalesOrderHeader soh)
        {
            return soh.StatusByte == 2;// OrderStatus.Approved;
        }

        internal static bool IsBackOrdered(this SalesOrderHeader soh)
        {
            return soh.StatusByte == 3; // OrderStatus.BackOrdered;
        }

        internal static bool IsRejected(this SalesOrderHeader soh)
        {
            return soh.StatusByte == 4; // OrderStatus.Rejected;
        }

        internal static bool IsShipped(this SalesOrderHeader soh)
        {
            return soh.StatusByte == 5; // OrderStatus.Shipped;
        }

        internal static bool IsCancelled(this SalesOrderHeader soh)
        {
            return soh.StatusByte == 6; // OrderStatus.Cancelled;
        }
        #endregion

        //TODO: Move to Edit
        public static ShipMethod DefaultShipMethod(this SalesOrderHeader soh, IContext context) => context.Instances<ShipMethod>().FirstOrDefault();


        public static string DisableDueDate(this SalesOrderHeader soh)
        {
            return soh.DisableIfShipped();
        }

        public static string ValidateDueDate(this SalesOrderHeader soh, DateTime dueDate)
        {
            if (dueDate.Date < soh.OrderDate.Date)
            {
                return "Due date cannot be before order date";
            }

            return null;
        }

        private static string DisableIfShipped(this SalesOrderHeader soh)
        {
            if (soh.IsShipped())
            {
                return "Order has been shipped";
            }
            return null;
        }

        public static string DisableShipDate(this SalesOrderHeader soh) => soh.DisableIfShipped();

        public static string DisableRecalculate(this SalesOrderHeader soh) =>
            !soh.IsInProcess() ? "Can only recalculate an 'In Process' order" : null;

        public static string ValidateShipDate(this SalesOrderHeader soh, DateTime? shipDate)
        {
            if (shipDate.HasValue && shipDate.Value.Date < soh.OrderDate.Date)
            {
                return "Ship date cannot be before order date";
            }

            return null;
        }

        #region Comments - all TODO
        //public static bool HideComment(this SalesOrderHeader soh)
        //{
        //    throw new NotImplementedException();
        //    //return Comment == null || Comment == "";
        //}

        //public static void ClearComment()
        //{
        //    throw new NotImplementedException();
        //    //this.Comment = null;
        //}

        //public static void AddStandardComments(IEnumerable<string> comments)
        //{
        //    throw new NotImplementedException();
        //    //foreach (string comment in comments)
        //    //{
        //    //    Comment += comment + "\n";
        //    //}
        //}

        //public IList<string> Choices0AddStandardComments()
        //{
        //    return new[] {
        //        "Payment on delivery",
        //        "Leave parcel with neighbour",
        //        "Leave parcel round back"
        //    };
        //}

        //public string[] Default0AddStandardComments()
        //{
        //    return new[] {
        //        "Payment on delivery",
        //        "Leave parcel with neighbour"
        //    };
        //}

        ////Action to demonstrate use of auto-complete that returns IEnumerable<string>
        //public void AddComment(string comment)
        //{
        //    Comment += comment + "\n";
        //}

        //[PageSize(10)]
        //public IEnumerable<string> AutoComplete0AddComment(
        //    [DescribedAs("Auto-complete")][NakedFunctions.Range(2, 0)] string matching)
        //{
        //    return Choices0AddStandardComments().Where(c => c.ToLower().Contains(matching.ToLower()));
        //}

        ////Action to demonstrate use of auto-complete that returns IEnumerable<string>
        //public void AddComment2(string comment)
        //{
        //    Comment += comment + "\n";
        //}

        //[PageSize(10)]
        //public IList<string> AutoComplete0AddComment2(
        //    [DescribedAs("Auto-complete")][NakedFunctions.Range(2, 0)] string matching)
        //{
        //    return Choices0AddStandardComments().Where(c => c.ToLower().Contains(matching.ToLower())).ToList();
        //}

        //public void AddMultiLineComment([MultiLine(NumberOfLines = 3)] string comment)
        //{
        //    Comment += comment + "\n";
        //}
        #endregion

        #region Edits - TODO
        //public static Address[] ChoicesBillingAddress(IContext context) => Person_MenuFunctions.AddressesFor(Customer.BusinessEntity(), context).ToList();

        //public static Address[] ChoicesShippingAddress(IContext context) => ChoicesBillingAddress(context);

    //    [PageSize(20)]
    //    public IQueryable<SalesPerson> AutoCompleteSalesPerson(
    //[MinLength(2)] string name, IContext context) =>
    //Sales_MenuFunctions.FindSalesPersonByName(null, name, context);



        #endregion
    }

}