// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using AW.Types;
using NakedFunctions;

namespace AW.Functions {
    [Named("Sales Order")]
    public static class SalesOrderHeader_Functions {
        internal static SalesOrderDetail CreateNewDetail(this SalesOrderHeader soh, Product product, short quantity, IContext context) {
            var specialOfferProduct = product.BestSpecialOfferProduct(quantity, context);
            return new SalesOrderDetail {
                SalesOrderHeader = soh,
                OrderQty = quantity,
                SpecialOfferProduct = specialOfferProduct,
                UnitPrice = product.ListPrice,
                UnitPriceDiscount = specialOfferProduct.SpecialOffer.DiscountPct,
                rowguid = context.NewGuid(),
                ModifiedDate = context.Now()
            };
        }

        internal static SalesOrderHeader Recalculated(this SalesOrderHeader soh, IContext c) {
            var subTotal = soh.Details.Any() ? soh.Details.Sum(d => d.LineTotal) : 0.0m;
            var tax = subTotal * soh.GetTaxRate(c) / 100;
            var total = subTotal + tax;
            return soh with {
                SubTotal = subTotal,
                TaxAmt = tax,
                TotalDue = total,
                ModifiedDate = c.Now()
            };
        }

        internal static decimal GetTaxRate(this SalesOrderHeader soh, IContext context) {
            var stateId = soh.BillingAddress.StateProvince.StateProvinceID;
            var str = context.Instances<SalesTaxRate>().FirstOrDefault(str => str.StateProvinceID == stateId);
            return str is null ? 0 : str.TaxRate;
        }

        [MemberOrder("Details", 2)]
        public static IContext AddCarrierTrackingNumber(this SalesOrderHeader soh,
                                                        IEnumerable<SalesOrderDetail> details, string ctn, IContext context) =>
            details.Select(d => new {
                       original = d,
                       updated = d with { CarrierTrackingNumber = ctn, ModifiedDate = context.Now() }
                   })
                   .Aggregate(context, (c, u) => c.WithUpdated(u.original, u.updated));

        [MemberOrder("Details", 3)] //Places action within the Details collection
        public static IContext ChangeAQuantity(this SalesOrderHeader soh,
                                               SalesOrderDetail detail, short newQuantity, IContext context) =>
            detail.ChangeQuantity(newQuantity, context);

        public static List<SalesOrderDetail> Choices1ChangeAQuantity(this SalesOrderHeader soh) =>
            soh.Details.ToList();

        public static string DisableChangeAQuantity(this SalesOrderHeader soh) =>
            soh.DisableAddNewDetail();

        internal static IContext UpdateOrder(
            SalesOrderHeader original, SalesOrderHeader updated, IContext context) =>
            context.WithUpdated(original, updated with { ModifiedDate = context.Now() });

        //TODO: Move to Edit
        public static ShipMethod DefaultShipMethod(this SalesOrderHeader soh, IContext context) => context.Instances<ShipMethod>().FirstOrDefault();

        public static string DisableDueDate(this SalesOrderHeader soh) => soh.DisableIfShipped();

        public static string ValidateDueDate(this SalesOrderHeader soh, DateTime dueDate) {
            if (dueDate.Date < soh.OrderDate.Date) {
                return "Due date cannot be before order date";
            }

            return null;
        }

        private static string DisableIfShipped(this SalesOrderHeader soh) {
            if (soh.IsShipped()) {
                return "Order has been shipped";
            }

            return null;
        }

        public static string DisableShipDate(this SalesOrderHeader soh) => soh.DisableIfShipped();

        public static string DisableRecalculate(this SalesOrderHeader soh) =>
            !soh.IsInProcess() ? "Can only recalculate an 'In Process' order" : null;

        public static string ValidateShipDate(this SalesOrderHeader soh, DateTime? shipDate) {
            if (shipDate.HasValue && shipDate.Value.Date < soh.OrderDate.Date) {
                return "Ship date cannot be before order date";
            }

            return null;
        }

        #region Add New Detail

        [MemberOrder(2)] [DescribedAs("Add a new line item to the order")]
        public static IContext AddNewDetail(
            this SalesOrderHeader soh,
            Product product,
            [DefaultValue(1)] [ValueRange(1, 999)] short quantity,
            IContext context
        ) {
            var sod = CreateNewDetail(soh, product, quantity, context);
            return context.WithNew(sod).WithDeferred(c => {
                var soh2 = c.Reload(soh);
                return c.WithUpdated(soh2, soh2.Recalculated(c));
            });
        }

        public static string DisableAddNewDetail(this SalesOrderHeader soh) {
            if (!soh.IsInProcess()) {
                return "Can only add to 'In Process' order";
            }

            return null;
        }

        [PageSize(20)]
        public static IQueryable<Product> AutoComplete1AddNewDetail(this SalesOrderHeader soh,
                                                                    [MinLength(2)] string name, IContext context) =>
            Product_MenuFunctions.FindProductByName(name, context);

        #endregion

        #region Remove Details

        public static IContext RemoveDetail(this SalesOrderHeader soh,
                                            SalesOrderDetail detailToRemove, IContext context) =>
            context.WithDeleted(detailToRemove)
                   .WithDeferred(c => c.WithUpdated(soh, soh.Recalculated(c)));

        public static IEnumerable<SalesOrderDetail> Choices1RemoveDetail(this SalesOrderHeader soh) =>
            soh.Details;

        public static SalesOrderDetail Default1RemoveDetail(this SalesOrderHeader soh) =>
            soh.Details.FirstOrDefault();

        public static string DisableRemoveDetail(this SalesOrderHeader soh) =>
            soh.Details.Any() ? null : "Order has no Details.";

        [MemberOrder("Details", 1)]
        public static IContext RemoveDetails(this SalesOrderHeader soh,
                                             IEnumerable<SalesOrderDetail> details, IContext context) =>
            details.Aggregate(context, (c, d) => c.WithDeleted(d))
                   .WithDeferred(c => c.WithUpdated(soh, soh.Recalculated(c)));

        #endregion

        #region ApproveOrder

        [MemberOrder(1)]
        public static IContext ApproveOrder(this SalesOrderHeader soh, IContext context) =>
            UpdateOrder(soh, soh with { StatusByte = (byte)OrderStatus.Approved }, context);

        //TODO: Remove context param from next 2
        public static bool HideApproveOrder(this SalesOrderHeader soh, IContext context) =>
            !soh.IsInProcess();

        public static string DisableApproveOrder(this SalesOrderHeader soh, IContext context) =>
            soh.Details.Count == 0 ? "Cannot approve orders with no Details" : null;

        #endregion

        #region Status helpers

        internal static bool IsInProcess(this SalesOrderHeader soh) => soh.StatusByte == 1; //OrderStatus.InProcess;

        internal static bool IsApproved(this SalesOrderHeader soh) => soh.StatusByte == 2; // OrderStatus.Approved;

        internal static bool IsBackOrdered(this SalesOrderHeader soh) => soh.StatusByte == 3; // OrderStatus.BackOrdered;

        internal static bool IsRejected(this SalesOrderHeader soh) => soh.StatusByte == 4; // OrderStatus.Rejected;

        internal static bool IsShipped(this SalesOrderHeader soh) => soh.StatusByte == 5; // OrderStatus.Shipped;

        internal static bool IsCancelled(this SalesOrderHeader soh) => soh.StatusByte == 6; // OrderStatus.Cancelled;

        #endregion
    }
}