using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class VendorContactMap : EntityTypeConfiguration<VendorContact>
    {
        public VendorContactMap()
        {
            // Primary Key
            this.HasKey(t => new { t.VendorID, t.ContactID });

            // Properties
            this.Property(t => t.VendorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("VendorContact", "Purchasing");
            this.Property(t => t.VendorID).HasColumnName("VendorID");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Contact).WithMany().HasForeignKey(t => t.ContactID);
            this.HasRequired(t => t.ContactType).WithMany().HasForeignKey(t => t.ContactTypeID);
            this.HasRequired(t => t.Vendor)
                .WithMany(t => t.Contacts)
                .HasForeignKey(d => d.VendorID);

        }
    }
}
