using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NOF2.Demo.Model
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmployeeDepartmentHistory> builder)
        {
            builder.HasKey(t => new { t.EmployeeID, t.DepartmentID, t.ShiftID, t.mappedStartDate });

            // Properties
            builder.Property(t => t.EmployeeID)
                   .ValueGeneratedNever();

            builder.Property(t => t.DepartmentID)
                   .ValueGeneratedNever();

            // Table & Column Mappings
            builder.ToTable("EmployeeDepartmentHistory", "HumanResources");
            builder.Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.DepartmentID).HasColumnName("DepartmentID");
            builder.Property(t => t.ShiftID).HasColumnName("ShiftID");
            builder.Property(t => t.mappedStartDate).HasColumnName("StartDate");
            builder.Property(t => t.mappedEndDate).HasColumnName("EndDate");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentID);
            builder.HasOne(t => t.Employee)
                   .WithMany(t => t.mappedDepartmentHistory)
                   .HasForeignKey(d => d.EmployeeID);
            builder.HasOne(t => t.Shift).WithMany().HasForeignKey(t => t.ShiftID);
        }
    }
}
