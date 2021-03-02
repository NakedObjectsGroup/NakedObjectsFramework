using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class UnitMeasureMap : EntityTypeConfiguration<UnitMeasure>
    {
        public UnitMeasureMap()
        {
            // Primary Key
            HasKey(t => t.UnitMeasureCode);

            // Properties
            Property(t => t.UnitMeasureCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("UnitMeasure", "Production");
            Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

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

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("UnitMeasure", "Production");
            builder.Property(t => t.UnitMeasureCode).HasColumnName("UnitMeasureCode");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
