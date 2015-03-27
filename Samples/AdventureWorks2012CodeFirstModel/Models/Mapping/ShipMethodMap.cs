using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class ShipMethodMap : EntityTypeConfiguration<ShipMethod>
    {
        public ShipMethodMap()
        {
            // Primary Key
            this.HasKey(t => t.ShipMethodID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ShipMethod", "Purchasing");
            this.Property(t => t.ShipMethodID).HasColumnName("ShipMethodID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.ShipBase).HasColumnName("ShipBase");
            this.Property(t => t.ShipRate).HasColumnName("ShipRate");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}
