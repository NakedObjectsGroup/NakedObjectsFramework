






using System;
using NakedFunctions;

namespace AW.Types {
    [Bounded]
    public class ProductSubcategory : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int ProductSubcategoryID { get; init; }

        public string Name { get; init; } = "";

        [Hidden]
        public int ProductCategoryID { get; init; }


        public virtual ProductCategory ProductCategory { get; init; }


        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => Name;


    }
}