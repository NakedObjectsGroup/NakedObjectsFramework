using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ScrapReason> builder)
        {
            builder.HasKey(t => t.ScrapReasonID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ScrapReason", "Production");
            builder.Property(t => t.ScrapReasonID).HasColumnName("ScrapReasonID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
