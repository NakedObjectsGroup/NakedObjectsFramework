






using System;
using System.Collections.Generic;
using AW.Functions;
using NakedFunctions;

namespace AW.Types {
    public class ProductModel : IHasRowGuid, IHasModifiedDate {
        [Hidden]
        public int ProductModelID { get; init; }

        [MemberOrder(10)]
        public string Name { get; init; } = "";

        [Hidden]
        public string? CatalogDescription { get; init; }

        [Named("CatalogDescription")] [MemberOrder(20)] [MultiLine(10)]
        public string FormattedCatalogDescription =>
            ProductModel_Functions.CatalogDescription(this);

        [MemberOrder(22)]
        public ProductDescription LocalCultureDescription =>
            ProductModel_Functions.LocalCultureDescription(this);

        [MemberOrder(30)]
        public string? Instructions { get; init; }

        [TableView(true, "Name", "Number", "Color", "ProductInventory")]
        public virtual ICollection<Product> ProductVariants { get; init; } = new List<Product>();

        [Hidden]
        public virtual ICollection<ProductModelIllustration> ProductModelIllustration { get; init; } = new List<ProductModelIllustration>();

        [Hidden]
        public virtual ICollection<ProductModelProductDescriptionCulture> ProductModelProductDescriptionCulture { get; init; } = new List<ProductModelProductDescriptionCulture>();

        
        [MemberOrder(99)]
        [Versioned]
        public DateTime ModifiedDate { get; init; }

        [Hidden]
        public Guid rowguid { get; init; }

        public override string ToString() => Name;


    }
}