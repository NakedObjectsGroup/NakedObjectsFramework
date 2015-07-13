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

namespace AdventureWorksModel {
    [DisplayName("Purchase Orders")]
    public class PurchaseOrderRepository : AbstractFactoryAndRepository {
        #region RandomPurchaseOrder

        [FinderAction]
        [QueryOnly]
        public PurchaseOrderHeader RandomPurchaseOrder() {
            return Random<PurchaseOrderHeader>();
        }

        #endregion

        [FinderAction]
        public PurchaseOrderHeader CreateNewPurchaseOrder([ContributedAction("Purchase Orders")] Vendor vendor) {
            var purchaseOrderHeader = NewTransientInstance<PurchaseOrderHeader>();
            purchaseOrderHeader.Vendor = vendor;
            //MakePersistent(_PurchaseOrderHeader);
            return purchaseOrderHeader;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0CreateNewPurchaseOrder([MinLength(2)] string name) {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        #region OpenPurchaseOrdersForVendor

        [TableView(true, "OrderDate", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendor([ContributedAction("Purchase Orders")] Vendor vendor) {
            return from obj in Instances<PurchaseOrderHeader>()
                where obj.Vendor.VendorID == vendor.VendorID && obj.Status <= 2
                select obj;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0OpenPurchaseOrdersForVendor([MinLength(2)] string name) {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region ListPurchaseOrdersForVendor

        [TableView(true, "OrderDate", "Status", "TotalDue")]
        public IQueryable<PurchaseOrderHeader> ListPurchaseOrders([ContributedAction("Purchase Orders")] Vendor vendor, DateTime? fromDate, DateTime? toDate) {
            IQueryable<PurchaseOrderHeader> query = from obj in Instances<PurchaseOrderHeader>()
                where obj.Vendor.VendorID == vendor.VendorID &&
                      (fromDate == null || obj.OrderDate >= fromDate) &&
                      (toDate == null || obj.OrderDate <= toDate)
                orderby obj.OrderDate
                select obj;

            return query;
        }

        [PageSize(20)]
        public IQueryable<Vendor> AutoComplete0ListPurchaseOrders([MinLength(2)] string name) {
            return Container.Instances<Vendor>().Where(v => v.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        #endregion

        #region OpenPurchaseOrdersForProduct

        [TableView(true, "Vendor", "OrderDate", "Status")]
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForProduct([ContributedAction("Purchase Orders")] Product product) {
            return from obj in Instances<PurchaseOrderDetail>()
                where obj.Product.ProductID == product.ProductID &&
                      obj.PurchaseOrderHeader.Status <= 2
                select obj.PurchaseOrderHeader;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0OpenPurchaseOrdersForProduct([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(product => product.Name.ToUpper().StartsWith(name.ToUpper()));
        }

        // for autoautocomplete testing
        public IQueryable<PurchaseOrderHeader> OpenPurchaseOrdersForVendorAndProduct(Vendor vendor, Product product) {

            return from obj in Instances<PurchaseOrderDetail>()
                   where obj.Product.ProductID == product.ProductID &&
                         obj.PurchaseOrderHeader.Status <= 2 && 
                         obj.PurchaseOrderHeader.Vendor.VendorID == vendor.VendorID
                   select obj.PurchaseOrderHeader;
        }


        #endregion

        
      #region FindById
      public PurchaseOrderHeader FindById(int id )
      {
      var query = from obj in Container.Instances<PurchaseOrderHeader>()
		              where obj.PurchaseOrderID == id
		              select obj;
                  
      return query.FirstOrDefault();
      }
      #endregion
          
    }
}