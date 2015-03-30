using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel.Models.Mapping
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            // Primary Key
            this.HasKey(t => t.EmployeeID);

            // Properties
            this.Property(t => t.NationalIDNumber)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.LoginID)
                .IsRequired()
                .HasMaxLength(256);

            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MaritalStatus)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.Gender)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("Employee", "HumanResources");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.NationalIDNumber).HasColumnName("NationalIDNumber");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.LoginID).HasColumnName("LoginID");
            this.Property(t => t.ManagerID).HasColumnName("ManagerID");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.HireDate).HasColumnName("HireDate");
            this.Property(t => t.SalariedFlag).HasColumnName("SalariedFlag");
            this.Property(t => t.VacationHours).HasColumnName("VacationHours");
            this.Property(t => t.SickLeaveHours).HasColumnName("SickLeaveHours");
            this.Property(t => t.CurrentFlag).HasColumnName("CurrentFlag");
            this.Property(t => t.rowguid).HasColumnName("rowguid");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasRequired(t => t.Contact)
                .WithMany(t => t.Employees)
                .HasForeignKey(d => d.ContactID);
            this.HasOptional(t => t.Employee2)
                .WithMany(t => t.Employee1)
                .HasForeignKey(d => d.ManagerID);

        }
    }
}
