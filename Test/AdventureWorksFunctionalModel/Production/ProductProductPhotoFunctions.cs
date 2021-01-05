using NakedFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksModel
{
    public static class ProductProductPhotoFunctions
    {
        #region Life Cycle Methods
        public static ProductProductPhoto Updating(this ProductProductPhoto x,  DateTime now) => x with { ModifiedDate = now };

        public static ProductProductPhoto Persisting(this ProductProductPhoto x,  DateTime now) => x with { ModifiedDate = now };
        #endregion
    }
}
