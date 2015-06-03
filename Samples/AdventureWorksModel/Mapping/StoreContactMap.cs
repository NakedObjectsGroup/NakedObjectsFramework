using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class StoreContactMap : EntityTypeConfiguration<StoreContact>
    {
        public StoreContactMap()
        {
            // Primary Key
            HasKey(t => new { t.CustomerID, t.ContactID });

            // Properties
            Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ContactID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("StoreContact", "Sales");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.ContactID).HasColumnName("ContactID");
            Property(t => t.ContactTypeID).HasColumnName("ContactTypeID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Contact).WithMany().HasForeignKey(t => t.ContactID);
            HasRequired(t => t.ContactType).WithMany().HasForeignKey(t => t.ContactTypeID);
            HasRequired(t => t.Store)
                .WithMany(t => t.Contacts)
                .HasForeignKey(d => d.CustomerID);

        }
    }
}
