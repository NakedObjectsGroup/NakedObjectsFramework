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
    [IconName("memo.png")]
    public class PurchaseOrderHeader  {
        #region Injected Services
        public IDomainObjectContainer Container { set; protected get; }
        public EmployeeRepository EmployeeRepository { set; protected get; }

        public ProductRepository ProductRepository { set; protected get; }
        #endregion

        #region Life Cycle Methods
        public void Created() {
            RevisionNumber = 0;
            Status = 1;
            OrderDate = DateTime.Today.Date;
        }

        public virtual void Persisting() {
            ModifiedDate = DateTime.Now;
        }

        public virtual void Updating() {
            byte increment = 1;
            RevisionNumber += increment;
            ModifiedDate = DateTime.Now;
        }
        #endregion

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

        #region ShipMethod

        [NakedObjectsIgnore]
        public virtual int ShipMethodID { get; set; }

        [MemberOrder(22)]
        public virtual ShipMethod ShipMethod { get; set; }

        public ShipMethod DefaultShipMethod()
        {
            return Container.Instances<ShipMethod>().First();
        }
        #endregion

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

        [Optionally]
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

        public Employee DefaultOrderPlacedBy() {
            return EmployeeRepository.RandomEmployee();
        }

        #endregion

        #region Details

        private ICollection<PurchaseOrderDetail> _details = new List<PurchaseOrderDetail>();

        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "OrderQty", "Product", "UnitPrice", "LineTotal")]
        public virtual ICollection<PurchaseOrderDetail> Details {
            get { return _details; }
            set { _details = value; }
        }

        #endregion

        #region Add New Detail

        [MemberOrder(1)]
        public PurchaseOrderDetail AddNewDetail(Product prod, short qty) {
            var pod = Container.NewTransientInstance<PurchaseOrderDetail>();
            pod.PurchaseOrderHeader = this;
            pod.Product = prod;
            pod.OrderQty = qty;
            return pod;
        }

        public virtual string DisableAddNewDetail() {
            if (!IsPending()) {
                return "Cannot add to Purchase Order unless status is Pending";
            }
            return null;
        }

        public List<Product> Choices0AddNewDetail() {
            return Vendor.Products.Select(n => n.Product).ToList();
        }
        #endregion

        #region Add New Details
        [MemberOrder(1)]
        [MultiLine()]
        public void AddNewDetails(Product prod, short qty, decimal unitPrice)
        {
            var det = AddNewDetail(prod, qty);
            det.UnitPrice = unitPrice;
            det.DueDate = DateTime.Today.AddDays(7);
            det.ReceivedQty = 0;
            det.RejectedQty = 0;
            Container.Persist(ref det);
        }

        [PageSize(10)]
        public IQueryable<Product> AutoComplete0AddNewDetails([MinLength(3)] string matching)
        {
            return ProductRepository.FindProductByName(matching);
        }

        #endregion

        #region Approve (Action)

        [MemberOrder(1)]
        public void Approve() {
            Status = 2;
        }

        public virtual bool HideApprove() {
            return !IsPending();
        }

        public virtual string DisableApprove() {
            var rb = new ReasonBuilder();
            if (Details.Count < 1) {
                rb.Append("Purchase Order must have at least one Detail to be approved");
            }
            return rb.Reason;
        }

        #endregion
    }
}