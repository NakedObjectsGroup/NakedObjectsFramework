using AW.Functions;
using NakedFramework.Value;

namespace AW.Types;

public class Product : IProduct, IHasModifiedDate, IHasRowGuid
{
    public Product() { }

    public Product(Product cloneFrom)
    {
        ProductID = cloneFrom.ProductID;
        Name = cloneFrom.Name;
        ProductNumber = cloneFrom.ProductNumber;
        Color = cloneFrom.Color;
        ProductModel = cloneFrom.ProductModel;
        ListPrice = cloneFrom.ListPrice;
        ProductSubcategory = cloneFrom.ProductSubcategory;
        ProductLine = cloneFrom.ProductLine;
        Style = cloneFrom.Style;
        Class = cloneFrom.Class;
        Make = cloneFrom.Make;
        FinishedGoods = cloneFrom.FinishedGoods;
        SafetyStockLevel = cloneFrom.SafetyStockLevel;
        ReorderPoint = cloneFrom.ReorderPoint;
        DaysToManufacture = cloneFrom.DaysToManufacture;
        SellStartDate = cloneFrom.SellStartDate;
        SellEndDate = cloneFrom.SellEndDate;
        DiscontinuedDate = cloneFrom.DiscontinuedDate;
        StandardCost = cloneFrom.StandardCost;
        Size = cloneFrom.Size;
        SizeUnitMeasureCode = cloneFrom.SizeUnitMeasureCode;
        SizeUnit = cloneFrom.SizeUnit;
        Weight = cloneFrom.Weight;
        WeightUnitMeasureCode = cloneFrom.WeightUnitMeasureCode;
        WeightUnit = cloneFrom.WeightUnit;
        ProductModelID = cloneFrom.ProductModelID;
        ProductSubcategoryID = cloneFrom.ProductSubcategoryID;
        rowguid = cloneFrom.rowguid;
        ModifiedDate = cloneFrom.ModifiedDate;
        ProductReviews = cloneFrom.ProductReviews;
        ProductInventory = cloneFrom.ProductInventory;
        ProductProductPhoto = cloneFrom.ProductProductPhoto;
        SpecialOfferProduct = cloneFrom.SpecialOfferProduct;
    }

    #region Visible properties

    [MemberOrder(1)]
    public string Name { get; init; } = "";

    [MemberOrder(2)]
    public string ProductNumber { get; init; } = "";

    [MemberOrder(3)]
    public string? Color { get; init; }

    [MemberOrder(4)]
    public virtual Image? Photo => Product_Functions.Photo(this);

    [MemberOrder(10)]
    public virtual ProductModel? ProductModel { get; init; }

    //MemberOrder 11 -  See Product_Functions.Description

    [MemberOrder(12)]
    [Mask("C")]
    public decimal ListPrice { get; init; }

    [MemberOrder(13)]
    public virtual ProductCategory? ProductCategory => this.ProductCategory();

    [MemberOrder(14)]
    public virtual ProductSubcategory? ProductSubcategory { get; init; }

    [MemberOrder(15)]
    public string? ProductLine { get; init; }

    [Named("Size")]
    [MemberOrder(16)]
    public string SizeWithUnit => this.SizeWithUnit();

    [Named("Weight")]
    [MemberOrder(17)]
    public string WeightWithUnit => this.WeightWithUnit();

    [MemberOrder(18)]
    public string? Style { get; init; }

    [MemberOrder(19)]
    public string? Class { get; init; }

    [MemberOrder(20)]
    public bool Make { get; init; }

    [MemberOrder(21)]
    public virtual bool FinishedGoods { get; init; }

    [MemberOrder(22)]
    public short SafetyStockLevel { get; init; }

    [MemberOrder(23)]
    public short ReorderPoint { get; init; }

    [MemberOrder(24)]
    public int DaysToManufacture { get; init; }

    [MemberOrder(81)]
    [Mask("d")]
    public DateTime SellStartDate { get; init; }

    [MemberOrder(82)]
    [Mask("d")]
    public DateTime? SellEndDate { get; init; }

    [MemberOrder(83)]
    [Mask("d")]
    public DateTime? DiscontinuedDate { get; init; }

    [MemberOrder(90)]
    [Mask("C")]
    public decimal StandardCost { get; init; }

    [MemberOrder(99)]
    [Versioned]
    public DateTime ModifiedDate { get; init; }

    #endregion

    #region Visible Collections

    [MemberOrder(100)]
    [TableView(true, nameof(ProductReview.Rating), nameof(ProductReview.Comments))]
    public virtual ICollection<ProductReview> ProductReviews { get; init; } = new List<ProductReview>();

    [MemberOrder(120)]
    [RenderEagerly]
    [TableView(false,
                                                  nameof(Types.ProductInventory.Quantity),
                                                  nameof(Types.ProductInventory.Location),
                                                  nameof(Types.ProductInventory.Shelf),
                                                  nameof(Types.ProductInventory.Bin))]
    public virtual ICollection<ProductInventory> ProductInventory { get; init; } = new List<ProductInventory>();

    #endregion

    #region Hidden Properties & Collections

    [Hidden]
    public int ProductID { get; init; }

    [Hidden]
    public string? Size { get; init; }

    [Hidden]
    public string? SizeUnitMeasureCode { get; init; }

    [Hidden]
    public virtual UnitMeasure? SizeUnit { get; init; }

    [Hidden]
    public decimal? Weight { get; init; }

    [Hidden]
    public string? WeightUnitMeasureCode { get; init; }

    [Hidden]
    public virtual UnitMeasure? WeightUnit { get; init; }

    [Hidden]
    public int? ProductModelID { get; init; }

    [Hidden]
    public int? ProductSubcategoryID { get; init; }

    [Hidden]
    public Guid rowguid { get; init; }

    public virtual ICollection<ProductProductPhoto> ProductProductPhoto { get; init; } = new List<ProductProductPhoto>();

    [Hidden]
    public virtual ICollection<SpecialOfferProduct> SpecialOfferProduct { get; init; } = new List<SpecialOfferProduct>();

    #endregion

    public override string ToString() => Name;
}