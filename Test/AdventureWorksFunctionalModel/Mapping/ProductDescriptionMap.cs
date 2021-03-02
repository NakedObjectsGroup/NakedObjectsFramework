using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class ProductDescriptionMap : EntityTypeConfiguration<ProductDescription>
    {
        public ProductDescriptionMap()
        {
            // Primary Key
            HasKey(t => t.ProductDescriptionID);

            // Properties
            Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            ToTable("ProductDescription", "Production");
            Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductDescription> builder)
        {
            builder.HasKey(t => t.ProductDescriptionID);

            // Properties
            builder.Property(t => t.Description)
                   .IsRequired()
                   .HasMaxLength(400);

            // Table & Column Mappings
            builder.ToTable("ProductDescription", "Production");
            builder.Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
