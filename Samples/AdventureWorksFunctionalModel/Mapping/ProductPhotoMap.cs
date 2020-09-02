using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class ProductPhotoMap : EntityTypeConfiguration<ProductPhoto>
    {
        public ProductPhotoMap()
        {
            // Primary Key
            HasKey(t => t.ProductPhotoID);

            // Properties
            Property(t => t.ThumbnailPhotoFileName)
                .HasMaxLength(50);

            Property(t => t.LargePhotoFileName)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductPhoto", "Production");
            Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            Property(t => t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto");
            Property(t => t.ThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName");
            Property(t => t.LargePhoto).HasColumnName("LargePhoto");
            Property(t => t.LargePhotoFileName).HasColumnName("LargePhotoFileName");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
