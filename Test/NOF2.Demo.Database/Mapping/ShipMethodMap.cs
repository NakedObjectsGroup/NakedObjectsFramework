using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<ShipMethod> builder)
        {
            builder.HasKey(t => t.ShipMethodID);

            // Properties
            builder.Property(t => t.mappedName)
                   .IsRequired()
                   .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("ShipMethod", "Purchasing");
            builder.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            builder.Property(t => t.mappedName).HasColumnName("Name");
            builder.Property(t => t.mappedShipBase).HasColumnName("ShipBase");
            builder.Property(t => t.mappedShipRate).HasColumnName("ShipRate");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);
        }
    }
}
