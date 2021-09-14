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
using static AW.Helpers;

namespace AW.Functions {
    public static class PurchaseOrderHeaderFunctions {
        //TODO Move to CreateNewPO
        public static Employee DefaultOrderPlacedBy(this PurchaseOrderHeader header, IContext context) =>
            Random<Employee>(context);

        //TODO ditto
        public static ShipMethod DefaultShipMethod(this PurchaseOrderHeader header, IContext context) =>
            context.Instances<ShipMethod>().First();

        internal static bool IsPending(this PurchaseOrderHeader poh) => poh.Status == 1;

        #region Add New Details

        [MemberOrder(1)] [MultiLine]
        public static IContext AddNewDetails(this PurchaseOrderHeader header,
                                             Product prod, short qty, decimal unitPrice, IContext context) =>
            context.WithNew(new PurchaseOrderDetail {
                PurchaseOrderHeader = header,
                Product = prod,
                OrderQty = qty,
                UnitPrice = unitPrice,
                DueDate = context.Today().Date.AddDays(7),
                ModifiedDate = context.Now()
            });

        [PageSize(10)]
        public static IQueryable<Product> AutoComplete1AddNewDetails(this PurchaseOrderHeader header,
                                                                     [MinLength(3)] string matching, IContext context) =>
            Product_MenuFunctions.FindProductByName(matching, context);

        #endregion

        #region Add New Detail

        [MemberOrder(1)]
        public static (PurchaseOrderDetail, IContext) AddNewDetail(this PurchaseOrderHeader header,
                                                                   Product prod, short qty, IContext context) {
            var pod = new PurchaseOrderDetail { PurchaseOrderHeader = header, Product = prod, OrderQty = qty };
            return (pod, context.WithNew(pod));
        }

        public static string DisableAddNewDetail(this PurchaseOrderHeader header) =>
            header.IsPending() ? null : "Cannot add to Purchase Order unless status is Pending";

        public static IList<Product> Choices1AddNewDetail(this PurchaseOrderHeader header) =>
            header.Vendor.Products.Select(n => n.Product).ToArray();

        #endregion

        #region Approve (Action)

        internal static IContext UpdatePOH(
            PurchaseOrderHeader original, PurchaseOrderHeader updated, IContext context) =>
            context.WithUpdated(original, updated with {
                ModifiedDate = context.Now(),
                RevisionNumber = Convert.ToByte(updated.RevisionNumber + 1)
            });

        [MemberOrder(1)]
        public static IContext Approve(this PurchaseOrderHeader po, IContext context) =>
            UpdatePOH(po, po with { Status = 2 }, context);

        public static bool HideApprove(this PurchaseOrderHeader po) => !po.IsPending();

        public static string DisableApprove(this PurchaseOrderHeader header) => header.Details.Count < 1 ? "Purchase Order must have at least one Detail to be approved" : null;

        #endregion
    }
}