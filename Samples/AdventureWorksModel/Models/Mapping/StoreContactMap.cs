using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class StoreContactMap : EntityTypeConfiguration<StoreContact>
    {
        public StoreContactMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CustomerID, t.ContactID });

            // Properties
            this.Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("StoreContact", "Sales");
            this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Contact)
                .WithMany(t => t.StoreContacts)
                .HasForeignKey(d => d.ContactID);
            this.HasRequired(t => t.ContactType)
                .WithMany(t => t.StoreContacts)
                .HasForeignKey(d => d.ContactTypeID);
            this.HasRequired(t => t.Store)
                .WithMany(t => t.StoreContacts)
                .HasForeignKey(d => d.CustomerID);

        }
    }
}
