using System.Data.Entity.ModelConfiguration;

namespace AdventureWorksModel
{
    public class JobCandidateMap : EntityTypeConfiguration<JobCandidate>
    {
        public JobCandidateMap()
        {
            // Primary Key
            HasKey(t => t.JobCandidateID);

            // Properties
            // Table & Column Mappings
            ToTable("JobCandidate", "HumanResources");
            Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            Property(t => t.Resume).HasColumnName("Resume");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            HasOptional(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeID);
        }
    }
}
