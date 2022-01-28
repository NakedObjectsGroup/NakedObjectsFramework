using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductModelProductDescriptionCulture> builder)
        {
            builder.HasKey(t => new { t.ProductModelID, t.ProductDescriptionID, t.CultureID });

            // Properties
            builder.Property(t => t.ProductModelID)
                   .ValueGeneratedNever();

            builder.Property(t => t.ProductDescriptionID)
                   .ValueGeneratedNever();

            builder.Property(t => t.CultureID)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(6);

            // Table & Column Mappings
            builder.ToTable("ProductModelProductDescriptionCulture", "Production");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            builder.Property(t => t.CultureID).HasColumnName("CultureID");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false); 

            // Relationships
            builder.HasOne(t => t.Culture).WithMany().HasForeignKey(t => t.CultureID);
            builder.HasOne(t => t.ProductDescription).WithMany().HasForeignKey(t => t.ProductDescriptionID);
            builder.HasOne(t => t.ProductModel)
                   .WithMany(t => t.ProductModelProductDescriptionCulture)
                   .HasForeignKey(d => d.ProductModelID);
        }
    }
}
