using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
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
            this.HasRequired(t => t.Contact);
            this.HasRequired(t => t.ContactType);
            this.HasRequired(t => t.Store)
                .WithMany(t => t.Contacts)
                .HasForeignKey(d => d.CustomerID);

        }
    }
}
