// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel
{
    [DisplayName("Purchase Orders")]
    public class PurchaseOrderRepository : AbstractFactoryAndRepository
    {

        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        #region RandomPurchaseOrder

        [Description("For demonstration purposes only")]
        [QueryOnly]
        [MemberOrder(1)]
        public PurchaseOrderHeader RandomPurchaseOrder()
        {
            return Random<PurchaseOrderHeader>();
        }

        #endregion

        #region All Purchase Orders
        //returns most recently-modified first
        [MemberOrder(2)]
        public IQueryable<PurchaseOrderHeader> AllPurchaseOrders()
        {
            return Container.Instances<PurchaseOrderHeader>().OrderByDescending(poh => poh.ModifiedDate);
        }
        #endregion

        #region All Open Purchase Order 
        [MemberOrder(2)]
        [TableView(true, "Vendor", "OrderDate", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> AllOpenPurchaseOrders()
        {
            return Container.Instances<PurchaseOrderHeader>().Where(poh => poh.Status <= 2);
        }

        #endregion

        #region OpenPurchaseOrdersForVendor

        [MemberOrder(3)]
        [TableView(true, "OrderDate", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendor([ContributedAction("Purchase Orders")] Vendor vendor)
        {
            return AllOpenPurchaseOrders().Where(poh => poh.Vendor.BusinessEntityID == vendor.BusinessEntityID);
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0OpenPurchaseOrdersForVendor([MinLength(2)] string name)
        {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region ListPurchaseOrdersForVendor

        [MemberOrder(4)]
        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> ListPurchaseOrders([ContributedAction("Purchase Orders")] Vendor vendor, DateTime? fromDate, DateTime? toDate)
        {
            IQueryable<PurchaseOrderHeader> query = from obj in Instances<PurchaseOrderHeader>()
                                                    where obj.Vendor.BusinessEntityID == vendor.BusinessEntityID &&
                                                          (fromDate == null || obj.OrderDate >= fromDate) &&
                                                          (toDate == null || obj.OrderDate <= toDate)
                                                    orderby obj.OrderDate
                                                    select obj;

            return query;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0ListPurchaseOrders([MinLength(2)] string name)
        {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        public string ValidateListPurchaseOrders(Vendor vendor, DateTime? fromDate, DateTime? toDate)
        {
            return toDate.IsAtLeastOneDayBefore(fromDate) ? "To Date cannot be before From Date" : null;
        }

        #endregion

        #region OpenPurchaseOrdersForProduct

        [MemberOrder(5)]
        [TableView(true, "Vendor", "OrderDate", "Status")]
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForProduct([ContributedAction("Purchase Orders")] Product product)
        {
            return from obj in Instances<PurchaseOrderDetail>()
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2
                   select obj.PurchaseOrderHeader;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0OpenPurchaseOrdersForProduct([MinLength(2)] string name)
        {
            return Container.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        // for autoautocomplete testing
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendorAndProduct(Vendor vendor, Product product)
        {

            return from obj in Instances<PurchaseOrderDetail>()
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2 &&
                         obj.PurchaseOrderHeader.Vendor.BusinessEntityID == vendor.BusinessEntityID
                   select obj.PurchaseOrderHeader;
        }


        #endregion

        #region Create New Purchase Order
        [MemberOrder(6)]
        public PurchaseOrderHeader CreateNewPurchaseOrder([ContributedAction("Purchase Orders")] Vendor vendor)
        {
            var purchaseOrderHeader = NewTransientInstance<PurchaseOrderHeader>();
            purchaseOrderHeader.Vendor = vendor;
            purchaseOrderHeader.ShipDate = DateTime.Today;
            return purchaseOrderHeader;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0CreateNewPurchaseOrder([MinLength(2)] string name)
        {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        [MemberOrder(7)]
        public PurchaseOrderHeader CreateNewPurchaseOrder2()
        {
            var purchaseOrderHeader = NewTransientInstance<PurchaseOrderHeader>();
            purchaseOrderHeader.OrderPlacedBy = null;
            return purchaseOrderHeader;
        }

        #endregion

        #region FindById
        [MemberOrder(7)]
        public PurchaseOrderHeader FindById(int id)
        {
            var query = from obj in Container.Instances<PurchaseOrderHeader>()
                        where obj.PurchaseOrderID == id
                        select obj;

            return query.FirstOrDefault();
        }
        #endregion
    }
}