using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ProductPhoto> builder)
        {
            builder.HasKey(t => t.ProductPhotoID);

            // Properties
            builder.Property(t => t.mappedThumbnailPhotoFileName)
                   .HasMaxLength(50);

            //builder.Property(t => t.LargePhotoFileName)
            //    .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ProductPhoto", "Production");
            builder.Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            builder.Property(t => t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto");
            builder.Property(t => t.mappedThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName");
            builder.Property(t => t.LargePhoto).HasColumnName("LargePhoto");
            //builder.Property(t => t.LargePhotoFileName).HasColumnName("LargePhotoFileName");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
