using System.Data.Entity.ModelConfiguration;
using AdventureWorksLegacyModel.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksLegacyModel.Mapping
{
    public class ShipMethodMap : EntityTypeConfiguration<ShipMethod>
    {
        public ShipMethodMap()
        {
            // Primary Key
            HasKey(t => t.ShipMethodID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ShipMethod", "Purchasing");
            Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ShipBase).HasColumnName("ShipBase");
            Property(t => t.ShipRate).HasColumnName("ShipRate");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ShipMethod> builder)
        {
            builder.HasKey(t => t.ShipMethodID);

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ShipMethod", "Purchasing");
            builder.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.ShipBase).HasColumnName("ShipBase");
            builder.Property(t => t.ShipRate).HasColumnName("ShipRate");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
