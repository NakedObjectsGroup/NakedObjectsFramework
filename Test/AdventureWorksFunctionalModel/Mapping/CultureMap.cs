using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping
{
    public class CultureMap : EntityTypeConfiguration<Culture>
    {
        public CultureMap()
        {
            // Primary Key
            HasKey(t => t.CultureID);

            // Properties
            Property(t => t.CultureID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Culture", "Production");
            Property(t => t.CultureID).HasColumnName("CultureID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }

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

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Culture", "Production");
            builder.Property(t => t.CultureID).HasColumnName("CultureID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");//.IsConcurrencyToken();
        }
    }
}
