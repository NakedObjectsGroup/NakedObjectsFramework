using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class EmployeeDepartmentHistoryMap : EntityTypeConfiguration<EmployeeDepartmentHistory>
    {
        public EmployeeDepartmentHistoryMap()
        {
            // Primary Key
            HasKey(t => new { t.EmployeeID, t.DepartmentID, t.ShiftID, t.StartDate });

            // Properties
            Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.DepartmentID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("EmployeeDepartmentHistory", "HumanResources");
            Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            Property(t => t.ShiftID).HasColumnName("ShiftID");
            Property(t => t.StartDate).HasColumnName("StartDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasRequired(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentID);
            HasRequired(t => t.Employee)
                .WithMany(t => t.DepartmentHistory)
                .HasForeignKey(d => d.EmployeeID);
            HasRequired(t => t.Shift).WithMany().HasForeignKey(t => t.ShiftID);

        }
    }
}
