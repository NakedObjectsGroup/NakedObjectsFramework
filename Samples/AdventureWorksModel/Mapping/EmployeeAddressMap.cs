using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class EmployeeAddressMap : EntityTypeConfiguration<EmployeeAddress>
    {
        public EmployeeAddressMap()
        {
            // Primary Key
            HasKey(t => new { t.EmployeeID, t.AddressID });

            // Properties
            Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("EmployeeAddress", "HumanResources");
            Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Employee);
                //.WithMany(t => t.Addresses)
                //.HasForeignKey(d => d.EmployeeID);
            HasRequired(t => t.Address).WithMany().HasForeignKey(t => t.AddressID);

        }
    }
}
