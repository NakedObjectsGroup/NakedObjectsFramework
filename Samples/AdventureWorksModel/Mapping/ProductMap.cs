using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            HasKey(t => t.ProductID);

            //Ignores
            Ignore(t => t.ProductCategory);
            Ignore(t => t.SpecialOffers);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ProductNumber)
                .IsRequired()
                .HasMaxLength(25);

            Property(t => t.Color)
                .HasMaxLength(15);

            Property(t => t.Size)
                .HasMaxLength(5);

            Property(t => t.SizeUnitMeasureCode)
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.WeightUnitMeasureCode)
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.ProductLine)
                .IsFixedLength()
                .HasMaxLength(2);

            Property(t => t.Class)
                .IsFixedLength()
                .HasMaxLength(2);

            Property(t => t.Style)
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            ToTable("Product", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ProductNumber).HasColumnName("ProductNumber");
            Property(t => t.Make).HasColumnName("MakeFlag");
            Property(t => t.FinishedGoods).HasColumnName("FinishedGoodsFlag");
            Property(t => t.Color).HasColumnName("Color");
            Property(t => t.SafetyStockLevel).HasColumnName("SafetyStockLevel");
            Property(t => t.ReorderPoint).HasColumnName("ReorderPoint");
            Property(t => t.StandardCost).HasColumnName("StandardCost");
            Property(t => t.ListPrice).HasColumnName("ListPrice");
            Property(t => t.Size).HasColumnName("Size");
            Property(t => t.SizeUnitMeasureCode).HasColumnName("SizeUnitMeasureCode");
            Property(t => t.WeightUnitMeasureCode).HasColumnName("WeightUnitMeasureCode");
            Property(t => t.Weight).HasColumnName("Weight");
            Property(t => t.DaysToManufacture).HasColumnName("DaysToManufacture");
            Property(t => t.ProductLine).HasColumnName("ProductLine");
            Property(t => t.Class).HasColumnName("Class");
            Property(t => t.Style).HasColumnName("Style");
            Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.SellStartDate).HasColumnName("SellStartDate");
            Property(t => t.SellEndDate).HasColumnName("SellEndDate");
            Property(t => t.DiscontinuedDate).HasColumnName("DiscontinuedDate");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasOptional(t => t.ProductModel)
                .WithMany(t => t.ProductVariants)
                .HasForeignKey(d => d.ProductModelID);
            HasOptional(t => t.ProductSubcategory).WithMany().HasForeignKey(t => t.ProductSubcategoryID); ;
            HasOptional(t => t.SizeUnit).WithMany().HasForeignKey(t => t.SizeUnitMeasureCode);
            HasOptional(t => t.WeightUnit).WithMany().HasForeignKey(t => t.WeightUnitMeasureCode);
        }
    }
}
