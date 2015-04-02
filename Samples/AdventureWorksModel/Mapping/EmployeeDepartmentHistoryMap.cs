using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class EmployeeDepartmentHistoryMap : EntityTypeConfiguration<EmployeeDepartmentHistory>
    {
        public EmployeeDepartmentHistoryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.EmployeeID, t.DepartmentID, t.ShiftID, t.StartDate });

            // Properties
            this.Property(t => t.EmployeeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DepartmentID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("EmployeeDepartmentHistory", "HumanResources");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            this.Property(t => t.ShiftID).HasColumnName("ShiftID");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentID);
            this.HasRequired(t => t.Employee)
                .WithMany(t => t.DepartmentHistory)
                .HasForeignKey(d => d.EmployeeID);
            this.HasRequired(t => t.Shift).WithMany().HasForeignKey(t => t.ShiftID);

        }
    }
}
