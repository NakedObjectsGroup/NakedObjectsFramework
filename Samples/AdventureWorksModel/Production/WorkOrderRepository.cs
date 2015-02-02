// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;

namespace AdventureWorksModel {
    [DisplayName("Work Orders")]
    public class WorkOrderRepository : AbstractFactoryAndRepository {
        #region Injected Services

        // This region should contain properties to hold references to any services required by the
        // object.  Use the 'injs' shortcut to add a new service.

        #endregion

        [FinderAction]
        [QueryOnly]
        public WorkOrder RandomWorkOrder() {
            return Random<WorkOrder>();
        }

        [FinderAction]
        public WorkOrder CreateNewWorkOrder([ContributedAction("Work Orders"), FindMenu]Product product) {
            var wo = NewTransientInstance<WorkOrder>();
            wo.Product = product;

            return wo;
        }

        [PageSize(20)]
        public IQueryable<Product> AutoComplete0CreateNewWorkOrder([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(p => p.Name.Contains(name));
        }

        #region CurrentWorkOrders

        [TableView(true, "Product", "OrderQty", "StartDate")]
        public IQueryable<WorkOrder> WorkOrders([ContributedAction("Work Orders")]Product product, bool currentOrdersOnly) {
            return from obj in Instances<WorkOrder>()
                where obj.Product.ProductID == product.ProductID &&
                      (currentOrdersOnly == false || obj.EndDate == null)
                select obj;
        }


        [PageSize(20)]
        public IQueryable<Product> AutoComplete0WorkOrders([MinLength(2)] string name) {
            return Container.Instances<Product>().Where(p => p.Name.Contains(name));
        }

        #endregion
    }
}