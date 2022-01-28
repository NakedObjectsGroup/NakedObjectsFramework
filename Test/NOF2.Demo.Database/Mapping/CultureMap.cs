using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Culture> builder)
        {
            builder.HasKey(t => t.CultureID);

            // Properties
            builder.Property(t => t.CultureID)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(6);

            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Culture", "Production");
            builder.Property(t => t.CultureID).HasColumnName("CultureID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
