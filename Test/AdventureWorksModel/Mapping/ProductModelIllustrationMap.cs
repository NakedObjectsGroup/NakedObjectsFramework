using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public class ProductModelIllustrationMap : EntityTypeConfiguration<ProductModelIllustration>
    {
        public ProductModelIllustrationMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductModelID, t.IllustrationID });

            // Properties
            Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.IllustrationID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductModelIllustration", "Production");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.IllustrationID).HasColumnName("IllustrationID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Illustration)
                .WithMany(t => t.ProductModelIllustration)
                .HasForeignKey(d => d.IllustrationID);
            HasRequired(t => t.ProductModel)
                .WithMany(t => t.ProductModelIllustration)
                .HasForeignKey(d => d.ProductModelID);

        }
    }

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
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Illustration)
                   .WithMany(t => t.ProductModelIllustration)
                   .HasForeignKey(d => d.IllustrationID);
            builder.HasOne(t => t.ProductModel)
                   .WithMany(t => t.ProductModelIllustration)
                   .HasForeignKey(d => d.ProductModelID);
        }
    }
}
