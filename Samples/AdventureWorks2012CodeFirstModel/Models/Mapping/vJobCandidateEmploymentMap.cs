using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class vJobCandidateEmploymentMap : EntityTypeConfiguration<vJobCandidateEmployment>
    {
        public vJobCandidateEmploymentMap()
        {
            // Primary Key
            this.HasKey(t => t.JobCandidateID);

            // Properties
            this.Property(t => t.Emp_OrgName)
                .HasMaxLength(100);

            this.Property(t => t.Emp_JobTitle)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("vJobCandidateEmployment", "HumanResources");
            this.Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            this.Property(t => t.Emp_StartDate).HasColumnName("Emp.StartDate");
            this.Property(t => t.Emp_EndDate).HasColumnName("Emp.EndDate");
            this.Property(t => t.Emp_OrgName).HasColumnName("Emp.OrgName");
            this.Property(t => t.Emp_JobTitle).HasColumnName("Emp.JobTitle");
            this.Property(t => t.Emp_Responsibility).HasColumnName("Emp.Responsibility");
            this.Property(t => t.Emp_FunctionCategory).HasColumnName("Emp.FunctionCategory");
            this.Property(t => t.Emp_IndustryCategory).HasColumnName("Emp.IndustryCategory");
            this.Property(t => t.Emp_Loc_CountryRegion).HasColumnName("Emp.Loc.CountryRegion");
            this.Property(t => t.Emp_Loc_State).HasColumnName("Emp.Loc.State");
            this.Property(t => t.Emp_Loc_City).HasColumnName("Emp.Loc.City");
        }
    }
}
