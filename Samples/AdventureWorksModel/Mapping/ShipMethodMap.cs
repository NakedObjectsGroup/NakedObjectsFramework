using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
}
