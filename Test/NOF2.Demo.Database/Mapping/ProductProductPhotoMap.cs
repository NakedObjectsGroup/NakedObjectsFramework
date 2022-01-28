using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductProductPhoto> builder)
        {
            builder.HasKey(t => new { t.ProductID, t.ProductPhotoID });

            // Properties
            builder.Property(t => t.ProductID)
                   .ValueGeneratedNever();

            builder.Property(t => t.ProductPhotoID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("ProductProductPhoto", "Production");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            builder.Property(t => t.mappedPrimary).HasColumnName("Primary");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.ProductProductPhoto)
                   .HasForeignKey(d => d.ProductID);
            builder.HasOne(t => t.ProductPhoto)
                   .WithMany(t => t.ProductProductPhoto)
                   .HasForeignKey(d => d.ProductPhotoID);
        }
    }
}
