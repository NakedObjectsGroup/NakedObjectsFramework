using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            // Primary Key
            HasKey(t => t.ProductCategoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductCategory", "Production");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductCategory> builder)
        {
            builder.HasKey(t => t.ProductCategoryID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ProductCategory", "Production");
            builder.Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
