// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedFunctions;
using static AdventureWorksModel.Helpers;


namespace AdventureWorksModel {
        public record PurchaseOrderHeader: IHasModifiedDate  {

        #region ID

        [Hidden]
        public virtual int PurchaseOrderID { get; init; }

        #endregion

        #region Revision Number

        
        [MemberOrder(90)]
        public virtual byte RevisionNumber { get; init; }

        #endregion

        [Hidden]
        public virtual int ShipMethodID { get; init; }

        [MemberOrder(22)]
        public virtual ShipMethod ShipMethod { get; init; }

        #region Vendor
        [Hidden]
        public virtual int VendorID { get; init; }

        [MemberOrder(1)]
        public virtual Vendor Vendor { get; init; }

        #endregion

        #region Status

        private static readonly string[] statusLabels = {"Pending", "Approved", "Rejected", "Complete"};

        [Hidden]
        [MemberOrder(10)]
        public virtual byte Status { get; init; }

        [Named("Status")]
        [MemberOrder(1)]
        public virtual string StatusAsString {
            get { return statusLabels[Status - 1]; }
        }

        #endregion

        #region Dates

        //Title
        [Mask("d")]
        [MemberOrder(11)]
        public virtual DateTime OrderDate { get; init; }

        [Mask("d")]
        [MemberOrder(20)]
        public virtual DateTime? ShipDate { get; init; }

        #endregion

        #region Amounts

        [MemberOrder(31)]
        
        [Mask("C")]
        public virtual decimal SubTotal { get; init; }

        
        [MemberOrder(32)]
        [Mask("C")]
        public virtual decimal TaxAmt { get; init; }

        
        [MemberOrder(33)]
        [Mask("C")]
        public virtual decimal Freight { get; init; }

        
        [MemberOrder(34)]
        [Mask("C")]
        public virtual decimal TotalDue { get; init; }

        #endregion

        #region Order Placed By (Employee)
        [Hidden]
        public virtual int OrderPlacedByID { get; init; }

        [MemberOrder(12)]
        public virtual Employee OrderPlacedBy { get; init; }
        #endregion

        [MemberOrder(99), ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; init; }

        [RenderEagerly]
        [TableView(true, "OrderQty", "Product", "UnitPrice", "LineTotal")]
        public virtual ICollection<PurchaseOrderDetail> Details { get; init; }
    }

    public static class PurchaseOrderHeaderFunctions
    {
        #region Add New Details
        [MemberOrder(1)]
        [MultiLine()]
        public static (object, PurchaseOrderDetail) AddNewDetails(
            PurchaseOrderHeader header, Product prod, short qty, decimal unitPrice, IContainer container)
        {
            var det = AddNewDetail(header, prod, qty, container);
            //TODO:  create new detail directly calling constructor with all params
            var det2 = det.Item1 with {UnitPrice =  unitPrice}
                 with {DueDate = container.Today().Date.AddDays(7)} 
                 with {ReceivedQty = 0}
                 with {RejectedQty =  0};
            return(null, det2);
        }

        [PageSize(10)]
        public static IQueryable<Product> AutoComplete0AddNewDetails(
            PurchaseOrderHeader header,
            [NakedFunctions.Range(3,0)] string matching,
            IContainer container)
        {
            return Product_MenuFunctions.FindProductByName(matching, container);
        }

        #endregion


        public static Employee DefaultOrderPlacedBy(
            PurchaseOrderHeader header,
            IQueryable<Employee> employees,
            [Injected] int random)
        {
            return Employee_MenuFunctions.RandomEmployee( employees, random);
        }

        public static ShipMethod DefaultShipMethod(
            PurchaseOrderHeader header,
            IQueryable<ShipMethod> shipMethods)
        {
            return shipMethods.First();
        }

        #region Add New Detail

        [MemberOrder(1)]
        public static (PurchaseOrderDetail, IContainer) AddNewDetail(
            PurchaseOrderHeader header, Product prod, short qty, IContainer container)
        {
            var pod = new PurchaseOrderDetail() { PurchaseOrderHeader = header, Product = prod, OrderQty = qty };
            return DisplayAndSave(pod, container);
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

        public static PurchaseOrderHeader Updating(
            PurchaseOrderHeader header,
            [Injected] DateTime now)
        {
            byte newRev = Convert.ToByte(header.RevisionNumber + 1);
            return header with {ModifiedDate =  now, RevisionNumber = newRev};
        }
        #endregion
    }
}