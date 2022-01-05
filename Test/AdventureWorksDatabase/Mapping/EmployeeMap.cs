using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{

    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(t => t.BusinessEntityID);

            // Ignores
            builder.Ignore(t => t.mappedNationalIDNumber)
                .Ignore(t => t.ManagerID)
                .Ignore(t => t.Manager)
                .Ignore(t => t.LoginID)
                .Ignore(t => t.JobTitle)
                .Ignore(t => t.DateOfBirth)
                .Ignore(t => t.MaritalStatus)
                .Ignore(t => t.Gender)
                .Ignore(t => t.HireDate)
                .Ignore(t => t.VacationHours)
                .Ignore(t => t.SickLeaveHours)
                .Ignore(t => t.mappedModifiedDate)
                .Ignore(t => t.Current)
                .Ignore(t => t.Salaried); 

            // Table & Column Mappings
            builder.ToTable("Employee", "HumanResources");
            builder.Property(t => t.BusinessEntityID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedNationalIDNumber).HasColumnName("NationalIDNumber").IsRequired().HasMaxLength(15);
            builder.Property(t => t.mappedLoginID).HasColumnName("LoginID").IsRequired().HasMaxLength(256);
            builder.Property(t => t.mappedJobTitle).HasColumnName("JobTitle").IsRequired().HasMaxLength(50);
            builder.Property(t => t.mappedDateOfBirth).HasColumnName("BirthDate");
            builder.Property(t => t.mappedMaritalStatus).HasColumnName("MaritalStatus").IsRequired().IsFixedLength().HasMaxLength(1);
            builder.Property(t => t.mappedGender).HasColumnName("Gender").IsRequired().IsFixedLength().HasMaxLength(1);
            builder.Property(t => t.mappedHireDate).HasColumnName("HireDate");
            builder.Property(t => t.mappedSalaried).HasColumnName("SalariedFlag");
            builder.Property(t => t.mappedVacationHours).HasColumnName("VacationHours");
            builder.Property(t => t.mappedSickLeaveHours).HasColumnName("SickLeaveHours");
            builder.Property(t => t.mappedCurrent).HasColumnName("CurrentFlag");
            builder.Property(t => t.RowGuid).HasColumnName("rowguid");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

           

            //.WithMany(t => t.DirectReports)
            //.HasForeignKey(d => d.ManagerID);
            //builder.HasOne(t => t.SalesPerson).WithOne(t => t.EmployeeDetails);
            builder.HasOne(t => t.PersonDetails).WithOne(t => t.Employee);
        }
    }
}
