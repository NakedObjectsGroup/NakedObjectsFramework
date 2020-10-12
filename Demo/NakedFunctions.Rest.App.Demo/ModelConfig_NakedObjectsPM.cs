using AdventureWorksModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedFunctions.Rest.App.Demo
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
                    //typeof(EntityCollection<object>),  TODO: What are this and next line for?
                   // typeof(ObjectQuery<object>),
                   //typeof(CustomerCollectionViewModel),
                   // //typeof(OrderLine),
                   // typeof(OrderStatus),
                   //// typeof(QuickOrderForm),
                   // typeof(ProductProductPhoto),
                   // typeof(ProductModelProductDescriptionCulture)
        };

        public static (string name, Type rootType)[] MainMenus() => new []
        {
            ("Customers", typeof(CustomerRepository)),
               // ("Orders", typeof(OrderRepository)),
               // ("Employees", typeof(EmployeeRepository)),
               // ("Persons", typeof(PersonRepository)),
                ("Vendors", typeof(VendorRepository)),
                //("Purchase Orders", typeof(PurchaseOrderRepository)),
                //("Work Orders", typeof(WorkOrderRepository))
        };

        public static Type[] Services() => new [] {
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
