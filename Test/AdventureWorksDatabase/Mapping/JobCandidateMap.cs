using System.Data.Entity.ModelConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorksModel
{
    public static partial class Mapper
    {
        public static void Map(this EntityTypeBuilder<JobCandidate> builder)
        {
            builder.HasKey(t => t.JobCandidateID);

            // Properties
            // Table & Column Mappings
            builder.ToTable("JobCandidate", "HumanResources");
            builder.Property(t => t.JobCandidateID).HasColumnName("JobCandidateID");
            builder.Property(t => t.EmployeeID).HasColumnName("BusinessEntityID");
            builder.Property(t => t.mappedResume).HasColumnName("Resume");
            builder.Property(t => t.mappedModifiedDate).HasColumnName("ModifiedDate").IsConcurrencyToken(false);

            // Relationships
            builder.HasOne(t => t.Employee).WithMany().HasForeignKey(t => t.EmployeeID);
        }
    }
}
