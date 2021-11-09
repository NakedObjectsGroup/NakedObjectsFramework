using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<EmployeeDepartmentHistory> builder)
        {
            builder.HasKey(t => new { t.EmployeeID, t.DepartmentID, t.ShiftID, t.StartDate });

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
            builder.Property(t => t.StartDate).HasColumnName("StartDate");
            builder.Property(t => t.EndDate).HasColumnName("EndDate");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Department).WithMany().HasForeignKey(t => t.DepartmentID);
            builder.HasOne(t => t.Employee)
                   .WithMany(t => t.DepartmentHistory)
                   .HasForeignKey(d => d.EmployeeID);
            builder.HasOne(t => t.Shift).WithMany().HasForeignKey(t => t.ShiftID);
        }
    }
}
