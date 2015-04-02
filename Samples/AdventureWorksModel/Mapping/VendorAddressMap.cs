using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class VendorAddressMap : EntityTypeConfiguration<VendorAddress>
    {
        public VendorAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { t.VendorID, t.AddressID });

            // Properties
            this.Property(t => t.VendorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("VendorAddress", "Purchasing");
            this.Property(t => t.VendorID).HasColumnName("VendorID");
            this.Property(t => t.AddressID).HasColumnName("AddressID");
            this.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            this.HasRequired(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            this.HasRequired(t => t.Vendor)
                .WithMany(t => t.Addresses)
                .HasForeignKey(d => d.VendorID);

        }
    }
}
