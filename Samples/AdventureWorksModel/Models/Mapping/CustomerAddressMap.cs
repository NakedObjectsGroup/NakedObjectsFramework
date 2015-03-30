using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CustomerAddressMap : EntityTypeConfiguration<CustomerAddress>
    {
        public CustomerAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { t.CustomerID, t.AddressID });

            // Properties
            this.Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("CustomerAddress", "Sales");
            this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.AddressID).HasColumnName("AddressID");
            this.Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Address);
            this.HasRequired(t => t.AddressType);
            this.HasRequired(t => t.Customer);
        }
    }
}
