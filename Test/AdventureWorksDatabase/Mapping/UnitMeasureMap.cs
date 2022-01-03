using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<UnitMeasure> builder)
        {
            builder.HasKey(t => t.UnitMeasureCode);

            // Properties
            builder.Property(t => t.UnitMeasureCode)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(3);

            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("UnitMeasure", "Production");
            builder.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
