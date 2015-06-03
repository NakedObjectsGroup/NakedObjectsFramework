using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class CustomerAddressMap : EntityTypeConfiguration<CustomerAddress>
    {
        public CustomerAddressMap()
        {
            // Primary Key
            HasKey(t => new { t.CustomerID, t.AddressID });

            // Properties
            Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("CustomerAddress", "Sales");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressTypeID).HasColumnName("AddressTypeID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);
            HasRequired(t => t.AddressType).WithMany().HasForeignKey(t => t.AddressTypeID);
            HasRequired(t => t.Customer).WithMany(c => c.Addresses).HasForeignKey(t => t.CustomerID);
        }
    }
}
