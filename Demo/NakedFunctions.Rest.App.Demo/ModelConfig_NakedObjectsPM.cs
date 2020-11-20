using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using NakedObjects.Menu;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedFunctions.Rest.App.Demo
{
    public class ModelConfig_NakedObjectsPM
    {
        public static Func<IConfiguration, DbContext> ContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static Type[] DomainTypes() => new[] {
            typeof(Vendor),
            typeof(Person),
            typeof(Customer),
            typeof(Shift),
            typeof(Store),
            typeof(BusinessEntity),
            typeof(EmployeeDepartmentHistory),
            typeof(Department),
            typeof(EmployeePayHistory),
            typeof(SpecialOfferProduct),
            typeof(CustomerDashboard),
            typeof(IEmployee)
        };

        public static IMenu[] MainMenus(IMenuFactory mf) => new[] {
            mf.NewMenu("Customers - NO", "customers_no", typeof(CustomerRepository), true),
            mf.NewMenu("Vendors - NO", "vendors_no", typeof(VendorRepository), true),
        };

        public static Type[] Services() => new [] {
          typeof(CustomerRepository),
          //          typeof(OrderRepository),
          //      typeof(ProductRepository),
          //          typeof(EmployeeRepository),
          //          typeof(SalesRepository),
          //          typeof(PersonRepository),
                    typeof(VendorRepository),
          //          typeof(PurchaseOrderRepository),
          //          typeof(WorkOrderRepository),
          //          typeof(OrderContributedActions),
          //          typeof(CustomerContributedActions)
        };
    }
}
