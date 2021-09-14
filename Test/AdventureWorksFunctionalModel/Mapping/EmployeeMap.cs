using System.Data.Entity.ModelConfiguration;
using AW.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AW.Mapping {
    public class EmployeeMap : EntityTypeConfiguration<Employee> {
        public EmployeeMap() {
            // Primary Key
            HasKey(t => t.BusinessEntityID);

            // Properties
            Property(t => t.NationalIDNumber)
                .IsRequired()
                .HasMaxLength(15);

            Property(t => t.LoginID)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.JobTitle)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.MaritalStatus)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            Property(t => t.Gender)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            ToTable("Employee", "HumanResources");
            Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            Property(t => t.NationalIDNumber).HasColumnName("NationalIDNumber");
            Property(t => t.LoginID).HasColumnName("LoginID");
            Property(t => t.JobTitle).HasColumnName("JobTitle");
            Property(t => t.DateOfBirth).HasColumnName("BirthDate");
            Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            Property(t => t.Gender).HasColumnName("Gender");
            Property(t => t.HireDate).HasColumnName("HireDate");
            Property(t => t.Salaried).HasColumnName("SalariedFlag");
            Property(t => t.VacationHours).HasColumnName("VacationHours");
            Property(t => t.SickLeaveHours).HasColumnName("SickLeaveHours");
            Property(t => t.Current).HasColumnName("CurrentFlag");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            Ignore(t => t.Manager);
            HasOptional(t => t.SalesPerson).WithRequired(t => t.EmployeeDetails);
            HasRequired(t => t.PersonDetails).WithOptional(t => t.Employee);
        }
    }

    public static partial class Mapper {
        public static void Map(this EntityTypeBuilder<Employee> builder) {
            builder.HasKey(t => t.BusinessEntityID);

            // Properties
            builder.Property(t => t.NationalIDNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(t => t.LoginID)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.Property(t => t.JobTitle)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(t => t.MaritalStatus)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(1);

            builder.Property(t => t.Gender)
                   .IsRequired()
                   .IsFixedLength()
                   .HasMaxLength(1);

            // Table & Column Mappings
            builder.ToTable("Employee", "HumanResources");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.NationalIDNumber).HasColumnName("NationalIDNumber");
            builder.Property(t => t.LoginID).HasColumnName("LoginID");
            builder.Property(t => t.JobTitle).HasColumnName("JobTitle");
            builder.Property(t => t.DateOfBirth).HasColumnName("BirthDate");
            builder.Property(t => t.MaritalStatus).HasColumnName("MaritalStatus");
            builder.Property(t => t.Gender).HasColumnName("Gender");
            builder.Property(t => t.HireDate).HasColumnName("HireDate");
            builder.Property(t => t.Salaried).HasColumnName("SalariedFlag");
            builder.Property(t => t.VacationHours).HasColumnName("VacationHours");
            builder.Property(t => t.SickLeaveHours).HasColumnName("SickLeaveHours");
            builder.Property(t => t.Current).HasColumnName("CurrentFlag");
            builder.Property(t => t.rowguid).HasColumnName("rowguid");
            builder.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate"); //.IsConcurrencyToken();

            // Relationships
            builder.Ignore(t => t.Manager);
            builder.HasOne(t => t.PersonDetails).WithOne(t => t.Employee);
        }
    }
}