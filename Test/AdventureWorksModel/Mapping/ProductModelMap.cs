using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class ProductModelMap : EntityTypeConfiguration<ProductModel>
    {
        public ProductModelMap()
        {
            // Primary Key
            HasKey(t => t.ProductModelID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductModel", "Production");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CatalogDescription).HasColumnName("CatalogDescription");
            Property(t => t.Instructions).HasColumnName("Instructions");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductModel> builder)
        {
            builder.HasKey(t => t.ProductModelID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ProductModel", "Production");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.CatalogDescription).HasColumnName("CatalogDescription");
            builder.Property(t => t.Instructions).HasColumnName("Instructions");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
