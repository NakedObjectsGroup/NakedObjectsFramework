using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AdventureWorks2012CodeFirstModel.Models.Mapping
{
    public class JobCandidateMap : EntityTypeConfiguration<JobCandidate>
    {
        public JobCandidateMap()
        {
            // Primary Key
            this.HasKey(t => t.JobCandidateID);

            // Properties
            // Table & Column Mappings
            this.ToTable("JobCandidate", "HumanResources");
            this.Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            this.Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            this.Property(t => t.Resume).HasColumnName("Resume");
            this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            this.HasOptional(t => t.Employee)
                .WithMany(t => t.JobCandidates)
                .HasForeignKey(d => d.EmployeeID);

        }
    }
}
