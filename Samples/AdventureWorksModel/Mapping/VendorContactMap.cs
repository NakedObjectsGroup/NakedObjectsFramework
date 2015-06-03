using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class VendorContactMap : EntityTypeConfiguration<VendorContact>
    {
        public VendorContactMap()
        {
            // Primary Key
            HasKey(t => new { t.VendorID, t.ContactID });

            // Properties
            Property(t => t.VendorID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("VendorContact", "Purchasing");
            Property(t => t.VendorID).HasColumnName("VendorID");
            Property(t => t.ContactID).HasColumnName("ContactID");
            Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Contact).WithMany().HasForeignKey(t => t.ContactID);
            HasRequired(t => t.ContactType).WithMany().HasForeignKey(t => t.ContactTypeID);
            HasRequired(t => t.Vendor)
                .WithMany(t => t.Contacts)
                .HasForeignKey(d => d.VendorID);

        }
    }
}
