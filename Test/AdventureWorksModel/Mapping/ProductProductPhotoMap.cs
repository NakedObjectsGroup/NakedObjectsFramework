using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class ProductProductPhotoMap : EntityTypeConfiguration<ProductProductPhoto>
    {
        public ProductProductPhotoMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.ProductPhotoID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductPhotoID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductProductPhoto", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            Property(t => t.Primary).HasColumnName("Primary");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product)
                .WithMany(t => t.ProductProductPhoto)
                .HasForeignKey(d => d.ProductID);
            HasRequired(t => t.ProductPhoto)
                .WithMany(t => t.ProductProductPhoto)
                .HasForeignKey(d => d.ProductPhotoID);

        }
    }

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
            builder.Property(t => t.Primary).HasColumnName("Primary");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

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
