using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class EmployeeAddressMap : EntityTypeConfiguration<EmployeeAddress>
    {
        public EmployeeAddressMap()
        {
            // Primary Key
            this.HasKey(t => new { t.EmployeeID, t.AddressID });

            // Properties
            this.Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("EmployeeAddress", "HumanResources");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.AddressID).HasColumnName("AddressID");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Employee)
                .WithMany(t => t.EmployeeAddresses)
                .HasForeignKey(d => d.EmployeeID);
            this.HasRequired(t => t.Address)
                .WithMany(t => t.EmployeeAddresses)
                .HasForeignKey(d => d.AddressID);

        }
    }
}
