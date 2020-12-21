
using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using static AdventureWorksModel.Helpers;

namespace AdventureWorksModel
{
   public static class Vendor_Functions
    {
        public static List<Product> ShowAllProducts(this Vendor vendor)
        {
            return vendor.Products.Select(vp => vp.Product).ToList();
        }

        public static (Person, IContext) CreateNewContact(this Vendor vendor, IContext context) =>
          DisplayAndSave(new Person() with { ForEntity = vendor }, context);

        [DescribedAs("Get report from credit agency")]
        public static Vendor CheckCredit(this Vendor v)
        {
            throw new NotImplementedException();
            //Not implemented.  Action is to test disable function only.
        }

        public static string DisableCheckCredit(this Vendor v) => "Not yet implemented";
    }
}
