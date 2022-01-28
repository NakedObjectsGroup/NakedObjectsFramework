using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasKey(t => t.ProductReviewID);

            // Properties
            builder.Property(t => t.mappedReviewerName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.mappedEmailAddress)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.mappedComments)
                   .HasMaxLength(3850);

            // Table & Column Mappings
            builder.ToTable("ProductReview", "Production");
            builder.Property(t => t.ProductReviewID).HasColumnName("ProductReviewID");
            builder.Property(t => t.ProductID).HasColumnName("ProductID");
            builder.Property(t => t.mappedReviewerName).HasColumnName("ReviewerName");
            builder.Property(t => t.mappedReviewDate).HasColumnName("ReviewDate");
            builder.Property(t => t.mappedEmailAddress).HasColumnName("EmailAddress");
            builder.Property(t => t.mappedRating).HasColumnName("Rating");
            builder.Property(t => t.mappedComments).HasColumnName("Comments");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Product)
                   .WithMany(t => t.mappedProductReviews)
                   .HasForeignKey(d => d.ProductID);
        }
    }
}
