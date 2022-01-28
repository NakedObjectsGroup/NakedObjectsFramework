using System.Data.Entity.ModelConfiguration;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(t => t.ProductID);

            builder.Ignore(t => t.ProductCategory);
            //builder.Ignore(t => t.mappedSpecialOffers);
            builder.Ignore(t => t.Container);


            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.mappedProductNumber)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(t => t.mappedColor)
                .HasMaxLength(15);

            builder.Property(t => t.mappedSize)
                .HasMaxLength(5);

            builder.Property(t => t.mappedSizeUnitMeasureCode)
                .IsFixedLength()
                .HasMaxLength(3);

            builder.Property(t => t.WeightUnitMeasureCode)
                .IsFixedLength()
                .HasMaxLength(3);

            builder.Property(t => t.mappedProductLine)
                .IsFixedLength()
                .HasMaxLength(2);

            builder.Property(t => t.mappedClass)
                .IsFixedLength()
                .HasMaxLength(2);

            builder.Property(t => t.mappedStyle)
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            builder.ToTable("Product", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedProductNumber).HasColumnName("ProductNumber");
            builder.Property(t => t.mappedMake).HasColumnName("MakeFlag");
            builder.Property(t => t.mappedFinishedGoods).HasColumnName("FinishedGoodsFlag");
            builder.Property(t => t.mappedColor).HasColumnName("Color");
            builder.Property(t => t.mappedSafetyStockLevel).HasColumnName("SafetyStockLevel");
            builder.Property(t => t.mappedReorderPoint).HasColumnName("ReorderPoint");
            builder.Property(t => t.mappedStandardCost).HasColumnName("StandardCost");
            builder.Property(t => t.mappedListPrice).HasColumnName("ListPrice");
            builder.Property(t => t.mappedSize).HasColumnName("Size");
            builder.Property(t => t.mappedSizeUnitMeasureCode).HasColumnName("SizeUnitMeasureCode");
            builder.Property(t => t.WeightUnitMeasureCode).HasColumnName("WeightUnitMeasureCode");
            builder.Property(t => t.Weight).HasColumnName("Weight");
            builder.Property(t => t.mappedDaysToManufacture).HasColumnName("DaysToManufacture");
            builder.Property(t => t.mappedProductLine).HasColumnName("ProductLine");
            builder.Property(t => t.mappedClass).HasColumnName("Class");
            builder.Property(t => t.mappedStyle).HasColumnName("Style");
            builder.Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.mappedSellStartDate).HasColumnName("SellStartDate");
            builder.Property(t => t.mappedSellEndDate).HasColumnName("SellEndDate");
            builder.Property(t => t.mappedDiscontinuedDate).HasColumnName("DiscontinuedDate");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.ProductModel)
                   .WithMany(t => t.mappedProductVariants)
                   .HasForeignKey(d => d.ProductModelID);
            builder.HasOne(t => t.ProductSubcategory).WithMany().HasForeignKey(t => t.ProductSubcategoryID); ;
            builder.HasOne(t => t.SizeUnit).WithMany().HasForeignKey(t => t.mappedSizeUnitMeasureCode);
            builder.HasOne(t => t.WeightUnit).WithMany().HasForeignKey(t => t.WeightUnitMeasureCode);
        }
    }
}
