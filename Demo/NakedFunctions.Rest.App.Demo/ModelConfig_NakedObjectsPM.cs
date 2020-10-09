using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedObjects.Rest.App.Demo
{
    public class ModelConfig_NakedObjectsPM
    {
        public static Func<IConfiguration, DbContext> ContextInstaller => c => new AdventureWorksContext(c.GetConnectionString("AdventureWorksContext"));

        public static string[] ModelNamespaces() => new[]
        {
            "AdventureWorksModel"
        };

        public static List<Type> DomainTypes() => new List<Type>
        {
                   //typeof(CustomerCollectionViewModel),
                   // //typeof(OrderLine),
                   // typeof(OrderStatus),
                   //// typeof(QuickOrderForm),
                   // typeof(ProductProductPhoto),
                   // typeof(ProductModelProductDescriptionCulture)
        };

        public static IList<(string name, Type rootType)> MainMenus() => new List<(string, Type)>
        {
            ("Customers", typeof(CustomerRepository)),
               // ("Orders", typeof(OrderRepository)),
               // ("Employees", typeof(EmployeeRepository)),
               // ("Persons", typeof(PersonRepository)),
                ("Vendors", typeof(VendorRepository)),
                //("Purchase Orders", typeof(PurchaseOrderRepository)),
                //("Work Orders", typeof(WorkOrderRepository))
        };

        public static List<Type> Services() => new List<Type> {
          typeof(CustomerRepository),
          //          typeof(OrderRepository),
          //          typeof(ProductRepository),
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
