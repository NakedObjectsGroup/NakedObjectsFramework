using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class VendorAddressMap : EntityTypeConfiguration<VendorAddress>
    {
        public VendorAddressMap()
        {
            // Primary Key
            HasKey(t => new { t.VendorID, t.AddressID });

            // Properties
            Property(t => t.VendorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("VendorAddress", "Purchasing");
            Property(t => t.VendorID).HasColumnName("VendorID");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            HasRequired(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            HasRequired(t => t.Vendor)
                .WithMany(t => t.Addresses)
                .HasForeignKey(d => d.VendorID);

        }
    }
}
