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
using NakedFunctions;
using NakedObjects;
using static NakedFunctions.Result;

namespace AdventureWorksModel {
    [IconName("memo.png")]
    public class PurchaseOrderHeader: IHasModifiedDate  {

        //TODO: Constructors & include work specified in old Created method (below) effectively as default values?

        #region ID

        [NakedObjectsIgnore]
        public virtual int PurchaseOrderID { get; set; }

        #endregion

        #region Revision Number

        [Disabled]
        [MemberOrder(90)]
        public virtual byte RevisionNumber { get; set; }

        #endregion

        #region ModifiedDate

        [MemberOrder(99)]
        [Disabled]
        [ConcurrencyCheck]
        public virtual DateTime ModifiedDate { get; set; }

        #endregion

        [NakedObjectsIgnore]
        public virtual int ShipMethodID { get; set; }

        [MemberOrder(22)]
        public virtual ShipMethod ShipMethod { get; set; }

        #region Vendor
        [NakedObjectsIgnore]
        public virtual int VendorID { get; set; }

        [Disabled(WhenTo.OncePersisted)]
        [MemberOrder(1)]
        public virtual Vendor Vendor { get; set; }

        #endregion

        #region Status

        private static readonly string[] statusLabels = {"Pending", "Approved", "Rejected", "Complete"};

        [NakedObjectsIgnore]
        [MemberOrder(10)]
        public virtual byte Status { get; set; }

        [DisplayName("Status")]
        [MemberOrder(1.1)]
        public virtual string StatusAsString {
            get { return statusLabels[Status - 1]; }
        }

        [NakedObjectsIgnore]
        public virtual bool IsPending() {
            return Status == 1;
        }

        #endregion

        #region Dates

        [Title]
        [Mask("d")]
        [MemberOrder(11)]
        public virtual DateTime OrderDate { get; set; }

        [Mask("d")]
        [MemberOrder(20)]
        public virtual DateTime? ShipDate { get; set; }

        #endregion

        #region Amounts

        [MemberOrder(31)]
        [Disabled]
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

        #endregion

        #region Order Placed By (Employee)
        [NakedObjectsIgnore]
        public virtual int OrderPlacedByID { get; set; }

        [MemberOrder(12)]
        public virtual Employee OrderPlacedBy { get; set; }
        #endregion

         [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "OrderQty", "Product", "UnitPrice", "LineTotal")]
        public virtual ICollection<PurchaseOrderDetail> Details { get; set; }
    }

    public static class PurchaseOrderHeaderFunctions
    {
        #region Add New Details
        [MemberOrder(1)]
        [MultiLine()]
        public static (object, PurchaseOrderDetail) AddNewDetails(
            PurchaseOrderHeader header,
            Product prod,
            short qty,
            decimal unitPrice,
            [Injected] DateTime now)
        {
            var det = AddNewDetail(header, prod, qty);
            //TODO:  create new detail directly calling constructor with all params
            var det2 = det.Item1.With(x => x.UnitPrice, unitPrice)
                .With(x => x.DueDate,now.Date.AddDays(7)) 
                .With(x => x.ReceivedQty,0)
                .With(x => x.RejectedQty, 0);
            return(null, det2);
        }

        [PageSize(10)]
        public static IQueryable<Product> AutoComplete0AddNewDetails(
            PurchaseOrderHeader header,
            [MinLength(3)] string matching,
            [Injected] IQueryable<Product> products)
        {
            return ProductRepository.FindProductByName(matching, products);
        }

        #endregion


        public static Employee DefaultOrderPlacedBy(
            PurchaseOrderHeader header,
            [Injected] IQueryable<Employee> employees,
            [Injected] int random)
        {
            return EmployeeRepository.RandomEmployee( employees, random);
        }

        public static ShipMethod DefaultShipMethod(
            PurchaseOrderHeader header,
            [Injected] IQueryable<ShipMethod> shipMethods)
        {
            return shipMethods.First();
        }

        #region Add New Detail

        [MemberOrder(1)]
        public static (PurchaseOrderDetail, PurchaseOrderDetail) AddNewDetail(
            PurchaseOrderHeader header,
            Product prod,
            short qty)
        {
            var pod = new PurchaseOrderDetail(header, prod, qty);
            return DisplayAndPersist(pod);
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
            var header2 = header.With(x => x.Status, 2);
            return DisplayAndPersist(header2);
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
            var newRev = header.RevisionNumber + 1;
            return header.With(x => x.ModifiedDate, now).With(x => x.RevisionNumber, newRev);
        }
        #endregion
    }
}