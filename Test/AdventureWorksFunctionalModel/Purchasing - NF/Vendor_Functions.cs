
using System.Collections.Generic;
using System.Linq;

namespace AdventureWorksModel
{
   public static class Vendor_Functions
    {
        //TODO: temporary test only - to show that a function can be contributed to an NO type
        public static List<Product> ShowAllProducts(this Vendor vendor)
        {
            return vendor.Products.Select(vp => vp.Product).ToList();
        }


    }
}
