using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            HasKey(t => t.ProductID);

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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasOptional(t => t.ProductModel)
                .WithMany(t => t.ProductVariants)
                .HasForeignKey(d => d.ProductModelID);
            HasOptional(t => t.ProductSubcategory).WithMany().HasForeignKey(t => t.ProductSubcategoryID); ;
            HasOptional(t => t.SizeUnit).WithMany().HasForeignKey(t => t.SizeUnitMeasureCode);
            HasOptional(t => t.WeightUnit).WithMany().HasForeignKey(t => t.WeightUnitMeasureCode);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(t => t.ProductID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.ProductNumber)
                .IsRequired()
                .HasMaxLength(25);

            builder.Property(t => t.Color)
                .HasMaxLength(15);

            builder.Property(t => t.Size)
                .HasMaxLength(5);

            builder.Property(t => t.SizeUnitMeasureCode)
                .IsFixedLength()
                .HasMaxLength(3);

            builder.Property(t => t.WeightUnitMeasureCode)
                .IsFixedLength()
                .HasMaxLength(3);

            builder.Property(t => t.ProductLine)
                .IsFixedLength()
                .HasMaxLength(2);

            builder.Property(t => t.Class)
                .IsFixedLength()
                .HasMaxLength(2);

            builder.Property(t => t.Style)
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            builder.ToTable("Product", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ProductNumber).HasColumnName("ProductNumber");
            builder.Property(t => t.Make).HasColumnName("MakeFlag");
            builder.Property(t => t.FinishedGoods).HasColumnName("FinishedGoodsFlag");
            builder.Property(t => t.Color).HasColumnName("Color");
            builder.Property(t => t.SafetyStockLevel).HasColumnName("SafetyStockLevel");
            builder.Property(t => t.ReorderPoint).HasColumnName("ReorderPoint");
            builder.Property(t => t.StandardCost).HasColumnName("StandardCost");
            builder.Property(t => t.ListPrice).HasColumnName("ListPrice");
            builder.Property(t => t.Size).HasColumnName("Size");
            builder.Property(t => t.SizeUnitMeasureCode).HasColumnName("SizeUnitMeasureCode");
            builder.Property(t => t.WeightUnitMeasureCode).HasColumnName("WeightUnitMeasureCode");
            builder.Property(t => t.Weight).HasColumnName("Weight");
            builder.Property(t => t.DaysToManufacture).HasColumnName("DaysToManufacture");
            builder.Property(t => t.ProductLine).HasColumnName("ProductLine");
            builder.Property(t => t.Class).HasColumnName("Class");
            builder.Property(t => t.Style).HasColumnName("Style");
            builder.Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.SellStartDate).HasColumnName("SellStartDate");
            builder.Property(t => t.SellEndDate).HasColumnName("SellEndDate");
            builder.Property(t => t.DiscontinuedDate).HasColumnName("DiscontinuedDate");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            //builder.HasOptional(t => t.ProductModel)
            //       .WithMany(t => t.ProductVariants)
            //       .HasForeignKey(d => d.ProductModelID);
            //builder.HasOptional(t => t.ProductSubcategory).WithMany().HasForeignKey(t => t.ProductSubcategoryID); ;
            //builder.HasOptional(t => t.SizeUnit).WithMany().HasForeignKey(t => t.SizeUnitMeasureCode);
            //builder.HasOptional(t => t.WeightUnit).WithMany().HasForeignKey(t => t.WeightUnitMeasureCode);
        }
    }
}
