using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
}
