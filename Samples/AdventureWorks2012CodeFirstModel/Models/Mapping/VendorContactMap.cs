using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
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
            this.HasRequired(t => t.Contact)
                .WithMany(t => t.VendorContacts)
                .HasForeignKey(d => d.ContactID);
            this.HasRequired(t => t.ContactType)
                .WithMany(t => t.VendorContacts)
                .HasForeignKey(d => d.ContactTypeID);
            this.HasRequired(t => t.Vendor)
                .WithMany(t => t.VendorContacts)
                .HasForeignKey(d => d.VendorID);

        }
    }
}
