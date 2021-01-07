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
using AW.Types;
using static AW.Helpers;


namespace AW.Functions {
        public static class PurchaseOrderHeaderFunctions
    {
        #region Add New Details
        [MemberOrder(1)]
        [MultiLine()]
        public static (object, PurchaseOrderDetail) AddNewDetails(
            PurchaseOrderHeader header, Product prod, short qty, decimal unitPrice, IContext context)
        {
            var det = AddNewDetail(header, prod, qty, context);
            //TODO:  create new detail directly calling constructor with all params
            var det2 = det.Item1 with {UnitPrice =  unitPrice}
                 with {DueDate = context.Today().Date.AddDays(7)} 
                 with {ReceivedQty = 0}
                 with {RejectedQty =  0};
            return(null, det2);
        }

        [PageSize(10)]
        public static IQueryable<Product> AutoComplete0AddNewDetails(
            PurchaseOrderHeader header,
            [NakedFunctions.Range(3,0)] string matching,
            IContext context)
        {
            return Product_MenuFunctions.FindProductByName(matching, context);
        }

        #endregion


        public static Employee DefaultOrderPlacedBy(this PurchaseOrderHeader header, IContext context) =>
           Random<Employee>(context);

        public static ShipMethod DefaultShipMethod(this PurchaseOrderHeader header, IContext context) =>
            context.Instances<ShipMethod>().First();
        

        #region Add New Detail

        [MemberOrder(1)]
        public static (PurchaseOrderDetail, IContext) AddNewDetail(
            PurchaseOrderHeader header, Product prod, short qty, IContext context)
        {
            var pod = new PurchaseOrderDetail() { PurchaseOrderHeader = header, Product = prod, OrderQty = qty };
            return DisplayAndSave(pod, context);
        }

        public static string DisableAddNewDetail(PurchaseOrderHeader header)
        {
            if (!header.IsPending())
            {
                return "Cannot add to Purchase Order unless status is Pending";
            }
            return null;
        }

        public static List<Product> Choices0AddNewDetail(PurchaseOrderHeader header)
        {
            return header.Vendor.Products.Select(n => n.Product).ToList();
        }
        #endregion

        #region Approve (Action)

        [MemberOrder(1)]
        public static (PurchaseOrderHeader, PurchaseOrderHeader) Approve(PurchaseOrderHeader header)
        {
            var header2 = header with {Status =  2};
            return (header2, header2);
        }

        public static bool HideApprove(PurchaseOrderHeader header)
        {
            return !header.IsPending();
        }

        public static string DisableApprove(PurchaseOrderHeader header)
        {
            return header.Details.Count < 1 ? "Purchase Order must have at least one Detail to be approved" : null;
        }

        #endregion


        internal static bool IsPending(this PurchaseOrderHeader poh) => poh.Status == 1;
 

        #region Life Cycle Methods

        //TODO: This work needs to be done in constructor
        public static void Created(PurchaseOrderHeader header)
        {
            //RevisionNumber = 0;
            //Status = 1;
            //OrderDate = DateTime.Today.Date;
        }

        public static PurchaseOrderHeader Updating(PurchaseOrderHeader x, IContext context)
        {
            byte newRev = Convert.ToByte(x.RevisionNumber + 1);
            return x with {ModifiedDate =  context.Now(), RevisionNumber = newRev};
        }
        #endregion
    }
}