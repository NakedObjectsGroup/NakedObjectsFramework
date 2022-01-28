using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductModelIllustration> builder)
        {
            builder.HasKey(t => new { t.ProductModelID, t.IllustrationID });

            // Properties
            builder.Property(t => t.ProductModelID)
                   .ValueGeneratedNever();

            builder.Property(t => t.IllustrationID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("ProductModelIllustration", "Production");
            builder.Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            builder.Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Illustration)
                   .WithMany(t => t.mappedProductModelIllustration)
                   .HasForeignKey(d => d.IllustrationID);
            builder.HasOne(t => t.ProductModel)
                   .WithMany(t => t.ProductModelIllustration)
                   .HasForeignKey(d => d.ProductModelID);
        }
    }
}
