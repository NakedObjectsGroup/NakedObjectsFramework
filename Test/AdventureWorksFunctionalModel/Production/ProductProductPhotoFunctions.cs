using NakedFunctions;
using AW.Types;

namespace AW.Functions
{
    public static class ProductProductPhotoFunctions
    {
        #region Life Cycle Methods
        public static ProductProductPhoto Updating(this ProductProductPhoto x,  IContext context) => x with { ModifiedDate = context.Now()};

        public static ProductProductPhoto Persisting(this ProductProductPhoto x,  IContext context) => x with { ModifiedDate = context.Now()};
        #endregion
    }
}
