using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class ProductSubcategoryMap : EntityTypeConfiguration<ProductSubcategory>
    {
        public ProductSubcategoryMap()
        {
            // Primary Key
            HasKey(t => t.ProductSubcategoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductSubcategory", "Production");
            Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
            HasRequired(t => t.ProductCategory)
                .WithMany(t => t.ProductSubcategory)
                .HasForeignKey(d => d.ProductCategoryID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductSubcategory> builder)
        {
            builder.HasKey(t => t.ProductSubcategoryID);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ProductSubcategory", "Production");
            builder.Property(t => t.ProductSubcategoryID).HasColumnName("ProductSubcategoryID");
            builder.Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();

            // Relationships
           //builder.HasRequired(t => t.ProductCategory)
           //        .WithMany(t => t.ProductSubcategory)
           //        .HasForeignKey(d => d.ProductCategoryID);
        }
    }
}
