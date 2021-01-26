using AW.Types;
using NakedFunctions;
using System.Linq;


namespace AW.Functions
{
     public static class QuickOrderLine_Functions
    {
        public static string[] DeriveKeys(this QuickOrderLine vm) =>
            new[] { vm.Product.ProductID.ToString(), vm.Number.ToString() };

        public static QuickOrderLine CreateFromKeys(string[] keys,IContext context)
        {
            int p = int.Parse(keys.First());
            short n = short.Parse(keys.Skip(1).First());
            var product = context.Instances<Product>().Single(c => c.ProductID == p);
            return new QuickOrderLine {Product =  product, Number= n};
        }


        public static (SalesOrderHeader, IContext) AddTo(this  QuickOrderLine vm, 
            SalesOrderHeader salesOrder, IContext context) =>
            salesOrder.AddNewDetail(vm.Product, vm.Number, context);
        
    }

}
