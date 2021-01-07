using NakedFunctions;
using AW.Types;

namespace AdventureWorksModel
{
   public static class ProductVendor_Functions
    {

        #region Life Cycle Methods
        public static ProductVendor Updating(ProductVendor pv, IContext context) => pv with { ModifiedDate = context.Now() };

        #endregion
    }
}
