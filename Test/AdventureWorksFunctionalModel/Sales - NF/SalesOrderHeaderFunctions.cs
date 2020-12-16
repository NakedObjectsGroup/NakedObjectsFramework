// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AdventureWorksFunctionalModel.NakedFunctions;
using NakedFunctions;

namespace AdventureWorksModel {
    [Named("Sales Order")]
        public  static class SalesOrderHeaderFunctions { 


        #region Actions

        #region Add New Detail

        [DescribedAs("Add a new line item to the order")]
#pragma warning disable 612,618
        [MemberOrder(2, "Details")]
#pragma warning restore 612,618
        public static (SalesOrderDetail, Action<IUserAdvisory>) AddNewDetail(
            this SalesOrderHeader soh,
            Product product,
            [DefaultValue((short) 1), Range(1, 999)] short quantity,
            IQueryable<SpecialOfferProduct> sops
            ) {
            int stock = product.NumberInStock();
            Action<IUserAdvisory> act = (IUserAdvisory ua) => ua.WarnUser(
            stock < quantity ? $"Current inventory of {product} is {stock}": "");
            var sod = new SalesOrderDetail
            {
                SalesOrderHeader = soh,
                SalesOrderID = soh.SalesOrderID,
                OrderQty = quantity,
                SpecialOfferProduct = ProductFunctions2.BestSpecialOfferProduct(product, quantity, sops)
            };
            //TODO:
            //sod.Recalculate();
            return (sod, act);
        }

        public static string DisableAddNewDetail(this SalesOrderHeader soh) {
            if (!soh.IsInProcess()) {
                return "Can only add to 'In Process' order";
            }
            return null;
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete0AddNewDetail(this SalesOrderHeader soh, [Range(2,0)] string name, IQueryable<Product> products) {
            return ProductRepository.FindProductByName( name, products);
        }
        #endregion

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
//        }
//        public virtual string DisableAddNewDetails()
//        {
//            return DisableAddNewDetail();
//        }
//        [PageSize(20)]
//        public IQueryable<Product> AutoComplete0AddNewDetails([Range(2,0)] string name, IQueryable<Product> products)
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

//        #region Remove Detail
//        public void RemoveDetail(SalesOrderDetail detailToRemove) {
//            if (Details.Contains(detailToRemove)) {
//                Details.Remove(detailToRemove);
//            }
//        }

//        public IEnumerable<SalesOrderDetail> Choices0RemoveDetail() {
//            return Details;
//        }

//        public SalesOrderDetail Default0RemoveDetail() {
//            return Details.FirstOrDefault();
//        }

//        [MemberOrder(3)]
//        public void RemoveDetails(this IEnumerable<SalesOrderDetail> details) {
//            foreach (SalesOrderDetail detail in details) {
//                if (Details.Contains(detail)) {
//                    Details.Remove(detail);
//                }
//            }
//        }
//        #endregion

//        #region AdjustQuantities
//        [MemberOrder(4)]
//        public void AdjustQuantities(this IEnumerable<SalesOrderDetail> details, short newQuantity)
//        {
//            foreach (SalesOrderDetail detail in details)
//            {
//                detail.OrderQty = newQuantity;
//            }
//        }

//        public string ValidateAdjustQuantities(IEnumerable<SalesOrderDetail> details, short newQuantity)
//        {
//            var rb = new ReasonBuilder();
//            rb.AppendOnCondition(details.Count(d => d.OrderQty == newQuantity) == details.Count(),
//                "All selected details already have specified quantity");
//            return rb.Reason;
//        }

//        #endregion

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


//        public IList<SalesOrderHeaderSalesReason> Choices0RemoveSalesReason()
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

//        #region ApproveOrder

//        [MemberOrder(1)]
//        public void ApproveOrder() {
//            Status = (byte) OrderStatus.Approved;
//        }

//        public virtual bool HideApproveOrder() {
//            return !IsInProcess();
//        }

//        public virtual string DisableApproveOrder() {
//            var rb = new ReasonBuilder();
//            if (Details.Count == 0) {
//                rb.Append("Cannot approve orders with no Details");
//            }
//            return rb.Reason;
//        }

//        #endregion

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

        #endregion
    }

}