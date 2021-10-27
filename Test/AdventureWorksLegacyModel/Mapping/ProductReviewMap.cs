using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class ProductReviewMap : EntityTypeConfiguration<ProductReview>
    {
        public ProductReviewMap()
        {
            // Primary Key
            HasKey(t => t.ProductReviewID);

            // Properties
            Property(t => t.ReviewerName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.EmailAddress)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Comments)
                .HasMaxLength(3850);

            // Table & Column Mappings
            ToTable("ProductReview", "Production");
            Property(t => t.ProductReviewID).HasColumnName("ProductReviewID");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.ReviewerName).HasColumnName("ReviewerName");
            Property(t => t.ReviewDate).HasColumnName("ReviewDate");
            Property(t => t.EmailAddress).HasColumnName("EmailAddress");
            Property(t => t.Rating).HasColumnName("Rating");
            Property(t => t.Comments).HasColumnName("Comments");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Product)
                .WithMany(t => t.ProductReviews)
                .HasForeignKey(d => d.ProductID);

        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(t => t.ProductReviewID);

            // Properties
            builder.Property(t => t.ReviewerName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.EmailAddress)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.Comments)
                   .HasMaxLength(3850);

            // Table & Column Mappings
            builder.ToTable("ProductReview", "Production");
            builder.Property(t => t.ProductReviewID).HasColumnName("ProductReviewID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.ReviewerName).HasColumnName("ReviewerName");
            builder.Property(t => t.ReviewDate).HasColumnName("ReviewDate");
            builder.Property(t => t.EmailAddress).HasColumnName("EmailAddress");
            builder.Property(t => t.Rating).HasColumnName("Rating");
            builder.Property(t => t.Comments).HasColumnName("Comments");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.ProductReviews)
                   .HasForeignKey(d => d.ProductID);
        }
    }
}
