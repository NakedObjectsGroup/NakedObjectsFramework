using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
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
            Ignore(t => t.ManagerID);//.HasColumnName("ManagerID");
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
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            Ignore(t => t.Manager);
                //.WithMany(t => t.DirectReports)
                //.HasForeignKey(d => d.ManagerID);
            HasOptional(t => t.SalesPerson).WithRequired(t => t.EmployeeDetails);
            HasRequired(t => t.PersonDetails).WithOptional(t => t.Employee);
        }
    }
}
