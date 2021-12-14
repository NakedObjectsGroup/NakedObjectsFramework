using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel.Mapping
{
    public class ProductDocumentMap : EntityTypeConfiguration<ProductDocument>
    {
        public ProductDocumentMap()
        {
            // Primary Key
            HasKey(t => new { t.ProductID, t.DocumentID });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.DocumentID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ProductDocument", "Production");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.DocumentID).HasColumnName("DocumentID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Document).WithMany().HasForeignKey(t => t.DocumentID);
            HasRequired(t => t.Product).WithMany().HasForeignKey(t => t.ProductID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductDocument> builder)
        {
            builder.HasKey(t => new { t.ProductID, t.DocumentID });

            // Properties
            //builder.Property(t => t.Description)
            //       .IsRequired()
            //       .HasMaxLength(400);

            // Table & Column Mappings
            builder.ToTable("ProductDocument", "Production");
            //builder.Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            //builder.Property(t => t.Description).HasColumnName("Description");
            //builder.Property(t => t.rowguid).HasColumnName("rowguid");
            //builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
